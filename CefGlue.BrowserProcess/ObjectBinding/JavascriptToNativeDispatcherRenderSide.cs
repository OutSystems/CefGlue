using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal record ObjectRegistration(ObjectInfo Info, string Messaging);

    internal class JavascriptToNativeDispatcherRenderSide : INativeObjectRegistry
    {
        private static volatile int lastCallId;

        private readonly object _registrationSyncRoot = new object();
        private readonly Dictionary<string, ObjectRegistration> _registeredObjects = new Dictionary<string, ObjectRegistration>();
        private readonly ConcurrentDictionary<int, PromiseHolder> _pendingCalls = new ConcurrentDictionary<int, PromiseHolder>();
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingBoundQueryTasks = new ConcurrentDictionary<string, TaskCompletionSource<bool>>();
        
        public JavascriptToNativeDispatcherRenderSide(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.NativeObjectRegistrationRequest.Name, HandleNativeObjectRegistration);
            dispatcher.RegisterMessageHandler(Messages.NativeObjectUnregistrationRequest.Name, HandleNativeObjectUnregistration);
            dispatcher.RegisterMessageHandler(Messages.NativeObjectCallResult.Name, HandleNativeObjectCallResult);

            JavascriptHelper.Register(this);
        }

        private void HandleNativeObjectRegistration(MessageReceivedEventArgs args)
        {
            var message = Messages.NativeObjectRegistrationRequest.FromCefMessage(args.Message);
            var registration = new ObjectRegistration(message.ObjectInfo, message.Messaging);

            lock (_registrationSyncRoot)
            {
                string objectName = registration.Info.Name;
                if (_registeredObjects.ContainsKey(objectName))
                {
                    return;
                }

                _registeredObjects.Add(objectName, registration);
                var taskSource = _pendingBoundQueryTasks.GetOrAdd(objectName, _ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));

                // register objects in the main frame
                var frame = args.Browser.GetMainFrame();
                var context = frame?.V8Context;

                if (context == null)
                {
                    // bail-out, lets try later when context is created
                    return;
                }

                var objectCreated = CreateNativeObjects(new [] { registration }, context);

                if (!objectCreated)
                {
                    taskSource.TrySetException(new Exception("Failed to create native object"));
                    return;
                }

                // notify that the object has been registered, any pending promises on the object will be resolved
                taskSource.TrySetResult(true);
            }
        }

        private void HandleNativeObjectUnregistration(MessageReceivedEventArgs args)
        {
            var message = Messages.NativeObjectUnregistrationRequest.FromCefMessage(args.Message);

            var frame = args.Browser.GetMainFrame(); // unregister objects from the main frame
            using (var context = frame.V8Context.EnterOrFail())
            {
                DeleteNativeObject(message.ObjectName, context.V8Context);
            }
        }

        private void HandleNativeObjectCallResult(MessageReceivedEventArgs args)
        {
            var message = Messages.NativeObjectCallResult.FromCefMessage(args.Message);
            if (_pendingCalls.TryRemove(message.CallId, out var promiseHolder))
            {
                using (promiseHolder)
                using (promiseHolder.Context.EnterOrFail())
                {
                    promiseHolder.ResolveOrReject((resolve, reject) =>
                    {
                        if (message.Success)
                        {
                            var value = CefV8Value.CreateArrayBuffer(message.Result);
                            resolve(value);
                        }
                        else
                        {
                            var exceptionMsg = CefV8Value.CreateString(message.Exception);
                            reject(exceptionMsg);
                        }
                    });
                }
            }
        }

        public void HandleContextCreated(CefV8Context context, bool isMain)
        { 
            if (isMain)
            {
                lock (_registrationSyncRoot)
                {
                    CreateNativeObjects(_registeredObjects.Values, context);
                }
            }
        }

        public void HandleContextReleased(CefV8Context context, bool isMain)
        {
            void ReleasePromiseHolder(PromiseHolder promiseHolder)
            {
                promiseHolder.Context.Dispose();
                promiseHolder.Dispose();
            }

            if (isMain)
            {
                foreach (var promiseHolder in _pendingCalls.Values)
                {
                    ReleasePromiseHolder(promiseHolder);
                }
                _pendingCalls.Clear();
            }
            else
            {
                foreach (var promiseHolderEntry in _pendingCalls.ToArray())
                {
                    if (promiseHolderEntry.Value.Context.IsSame(context))
                    {
                        _pendingCalls.TryRemove(promiseHolderEntry.Key, out var dummy);
                        ReleasePromiseHolder(promiseHolderEntry.Value);
                    }
                }
            }
        }

        private bool CreateNativeObjects(IEnumerable<ObjectRegistration> objectRegistrations, CefV8Context context)
        {
            if (context.Enter())
            {
                try
                {
                    var global = context.GetGlobal();
                    foreach (var registration in objectRegistrations)
                    {
                        ObjectInfo objectInfo = registration.Info;

                        var handler = new V8FunctionHandler((string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception) =>
                        {
                            if (arguments.Length > 1)
                            {
                                throw new ArgumentException($"The array must be either empty or contain only one argument, a json string. The array has {arguments.Length} elements.", nameof(arguments));
                            }

                            using var context = CefV8Context.GetCurrentContext().EnterOrFail(shallDispose: false); // context will be released when promise is resolved
                            var message = new Messages.NativeObjectCallRequest()
                            {
                                ObjectName = objectInfo.Name,
                                CallId = lastCallId++,
                                MemberName = name,
                                Arguments = arguments.FirstOrDefault()?.GetArrayBuffer() ?? []
                            };

                            var frame = context.V8Context.GetFrame();
                            if (frame == null)
                            {
                                // TODO, what now?
                                returnValue = null;
                                exception = "Failed to create promise";
                            }

                            CefV8Value serializer = obj.GetValue("serializer");
                            PromiseHolder promiseHolder = context.V8Context.CreatePromise(serializer);
                            if (!_pendingCalls.TryAdd(message.CallId, promiseHolder))
                            {
                                throw new InvalidOperationException("Call id already exists");
                            }

                            var cefMessage = message.ToCefProcessMessage();
                            frame.SendProcessMessage(CefProcessId.Browser, cefMessage);

                            returnValue = promiseHolder.Promise;
                            exception = null;

                            return true;
                        });

                        CefV8Value serializer = registration.Messaging != null
                            ? global.GetGlueValue(registration.Messaging)
                            : CefV8Value.CreateUndefined();

                        var v8Obj = CefV8Value.CreateObject();
                        v8Obj.SetValue("serializer", serializer, CefV8PropertyAttribute.ReadOnly);

                        foreach (var method in objectInfo.Methods)
                        {
                            var v8Function = CefV8Value.CreateFunction(method.Name, handler);
                            var result = v8Function.SetValue("length", CefV8Value.CreateInt(method.ParameterCount), CefV8PropertyAttribute.ReadOnly);
                            v8Obj.SetValue(method.Name, v8Function);
                        }

                        // the interceptor object is a proxy that will trap all calls to the native object and
                        // and pass the arguments serialized as json (to the native method)
                        var interceptorObj = JavascriptHelper.CreateInterceptorObject(context, v8Obj, serializer);
                        global.SetValue(objectInfo.Name, interceptorObj);
                    }

                    return true;
                }
                finally
                {
                    context.Exit();
                }
            }
            else
            {
                // TODO
                return false;
            }
        }

        private void DeleteNativeObject(string objName, CefV8Context context)
        {
            lock (_registrationSyncRoot)
            {
                if (_registeredObjects.Remove(objName))
                {
                    var global = context.GetGlobal();
                    global.DeleteValue(objName);
                }
            }
        }

        Task<bool> INativeObjectRegistry.Bind(string objName)
        {
            return _pendingBoundQueryTasks.GetOrAdd(objName, _ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)).Task;
        }

        void INativeObjectRegistry.Unbind(string objName)
        {
            using (var context = CefV8Context.GetCurrentContext().EnterOrFail())
            {
                DeleteNativeObject(objName, context.V8Context);
            }
        }
    }
}
