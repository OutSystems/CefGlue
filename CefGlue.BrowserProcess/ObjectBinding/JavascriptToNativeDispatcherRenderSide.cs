using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xilium.CefGlue.BrowserProcess.Serialization;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal class JavascriptToNativeDispatcherRenderSide : INativeObjectRegistry
    {
        private static volatile int lastCallId;

        private readonly ConcurrentDictionary<string, ObjectRegistrationInfo> _registeredObjects = new ConcurrentDictionary<string, ObjectRegistrationInfo>();
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
            var browser = args.Browser;
            var context = browser.GetMainFrame().V8Context;

            var message = Messages.NativeObjectRegistrationRequest.FromCefMessage(args.Message);
            var objectInfo = new ObjectRegistrationInfo(message.ObjectName, message.MethodsNames);

            if (_registeredObjects.TryAdd(objectInfo.Name, objectInfo))
            {
                var objectCreated = CreateNativeObject(objectInfo, context);

                if (objectCreated)
                {
                    // notify that the object has been registered, any pending promises on the object will be resolved
                    var taskSource = _pendingBoundQueryTasks.GetOrAdd(objectInfo.Name, _ => new TaskCompletionSource<bool>());
                    taskSource.TrySetResult(true);
                }
            }
        }

        private void HandleNativeObjectUnregistration(MessageReceivedEventArgs args)
        {
            var browser = args.Browser;
            var context = browser.GetMainFrame().V8Context;

            var message = Messages.NativeObjectUnregistrationRequest.FromCefMessage(args.Message);

            DeleteNativeObject(message.ObjectName, context);
        }

        private PromiseHolder HandleNativeObjectCall(Messages.NativeObjectCallRequest message)
        {
            message.CallId = lastCallId++;

            var promiseHolder = JavascriptHelper.CreatePromise();
            if (!_pendingCalls.TryAdd(message.CallId, promiseHolder))
            {
                throw new InvalidOperationException("Call id already exists");
            }

            var browser = CefV8Context.GetCurrentContext().GetBrowser();
            using (var cefMessage = message.ToCefProcessMessage())
            {
                browser.SendProcessMessage(CefProcessId.Browser, cefMessage);
            }

            return promiseHolder;
        }

        private void HandleNativeObjectCallResult(MessageReceivedEventArgs args)
        {
            var message = Messages.NativeObjectCallResult.FromCefMessage(args.Message);
            if(_pendingCalls.TryRemove(message.CallId, out var promiseHolder))
            {
                promiseHolder.ResolveOrReject((resolve, reject) =>
                {
                    if (message.Success)
                    {
                        resolve(V8ValueSerialization.SerializeCefValue(message.Result));
                    }
                    else
                    {
                        reject(CefV8Value.CreateString(message.Exception));
                    }
                });
            }
            else
            {
                // probably the call context has gone, bail out
            }
        }

        public void HandleContextCreated(CefV8Context context, bool isMain)
        {
            if (isMain)
            {
                foreach (var obj in _registeredObjects.Values)
                {
                    CreateNativeObject(obj, context);
                }
            }
        }

        public void HandleContextReleased(CefV8Context context, bool isMain)
        {
            if (isMain)
            {
                _pendingBoundQueryTasks.Clear();
                _pendingCalls.Clear();
            }
            else
            {
                foreach (var promiseHolderEntry in _pendingCalls.ToArray())
                {
                    if (promiseHolderEntry.Value.Context == context)
                    {
                        _pendingCalls.TryRemove(promiseHolderEntry.Key, out var dummy);
                    }
                }
            }
        }

        private bool CreateNativeObject(ObjectRegistrationInfo objectInfo, CefV8Context context)
        {
            if (context.Enter())
            {
                try
                {
                    var global = context.GetGlobal();
                    var handler = new V8FunctionHandler(objectInfo.Name, HandleNativeObjectCall);
                    var attributes = CefV8PropertyAttribute.ReadOnly | CefV8PropertyAttribute.DontEnum | CefV8PropertyAttribute.DontDelete;

                    using (var v8Obj = CefV8Value.CreateObject())
                    {
                        foreach (var methodName in objectInfo.MethodsNames)
                        {
                            using (var v8Function = CefV8Value.CreateFunction(methodName, handler))
                            {
                                v8Obj.SetValue(methodName, v8Function, attributes);
                            }
                        }

                        global.SetValue(objectInfo.Name, v8Obj);
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
            if (_registeredObjects.TryRemove(objName, out var objectInfo))
            {
                DeleteNativeObject(objectInfo, context);
            }
        }

        private void DeleteNativeObject(ObjectRegistrationInfo objectInfo, CefV8Context context)
        {
            if (context.Enter())
            {
                try
                {
                    var global = context.GetGlobal();
                    global.DeleteValue(objectInfo.Name);
                }
                finally
                {
                    context.Exit();
                }
            }
            else
            {
                // TODO
            }
        }

        Task<bool> INativeObjectRegistry.Bind(string objName)
        {
            return _pendingBoundQueryTasks.GetOrAdd(objName, _ => new TaskCompletionSource<bool>()).Task;
        }

        void INativeObjectRegistry.Unbind(string objName)
        {
            DeleteNativeObject(objName, CefV8Context.GetCurrentContext());
        }
    }
}
