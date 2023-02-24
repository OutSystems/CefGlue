using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal class JavascriptToNativeDispatcherRenderSide : INativeObjectRegistry
    {
        private static volatile int lastCallId;

        private readonly object _registrationSyncRoot = new object();
        private readonly Dictionary<string, ObjectRegistrationInfo> _registeredObjects = new Dictionary<string, ObjectRegistrationInfo>();
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
            var objectInfo = new ObjectRegistrationInfo(message.ObjectName, message.MethodsNames);

            lock (_registrationSyncRoot)
            {
                if (_registeredObjects.ContainsKey(objectInfo.Name))
                {
                    return;
                }

                _registeredObjects.Add(objectInfo.Name, objectInfo);
                var taskSource = _pendingBoundQueryTasks.GetOrAdd(objectInfo.Name, _ => new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously));

                // register objects in the main frame
                var frame = args.Browser.GetMainFrame();
                var context = frame?.V8Context;

                if (context == null)
                {
                    // bail-out, lets try later when context is created
                    return;
                }

                var objectCreated = CreateNativeObjects(new[] { objectInfo }, context);

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

        private PromiseHolder HandleNativeObjectCall(Messages.NativeObjectCallRequest message)
        {
            message.CallId = lastCallId++;

            using (var context = CefV8Context.GetCurrentContext().EnterOrFail(shallDispose: false)) // context will be released when promise is resolved
            {
                var frame = context.V8Context.GetFrame();
                if (frame == null)
                {
                    // TODO, what now?
                    return null;
                }

                var promiseHolder = context.V8Context.CreatePromise();
                if (!_pendingCalls.TryAdd(message.CallId, promiseHolder))
                {
                    throw new InvalidOperationException("Call id already exists");
                }

                var cefMessage = message.ToCefProcessMessage();
                frame.SendProcessMessage(CefProcessId.Browser, cefMessage);

                return promiseHolder;
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
                            var value = CefV8Value.CreateString(message.ResultAsJson);
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

        private bool CreateNativeObjects(IEnumerable<ObjectRegistrationInfo> objectInfos, CefV8Context context)
        {
            if (context.Enter())
            {
                try
                {
                    var global = context.GetGlobal();
                    foreach (var objectInfo in objectInfos)
                    {
                        var handler = new V8FunctionHandler(objectInfo.Name, HandleNativeObjectCall);
                        
                        var v8Obj = CefV8Value.CreateObject();
                        foreach (var methodName in objectInfo.MethodsNames)
                        {
                            var v8Function = CefV8Value.CreateFunction(methodName, handler);
                            v8Obj.SetValue(methodName, v8Function);
                        }

                        // the interceptor object is a proxy that will trap all calls to the native object and
                        // and pass the arguments serialized as json (to the native method)
                        var interceptorObj = JavascriptHelper.CreateInterceptorObject(context, v8Obj);
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
