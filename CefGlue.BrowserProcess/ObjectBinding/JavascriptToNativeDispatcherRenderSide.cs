using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xilium.CefGlue.BrowserProcess.Serialization;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal class JavascriptToNativeDispatcherRenderSide
    {
        private static volatile int lastCallId;

        private readonly ConcurrentDictionary<int, PromiseHolder> _pendingCalls = new ConcurrentDictionary<int, PromiseHolder>();
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingBoundQueryTasks = new ConcurrentDictionary<string, TaskCompletionSource<bool>>();

        private readonly JavascriptHelper _javascriptHelper = new JavascriptHelper();
        
        public JavascriptToNativeDispatcherRenderSide(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.NativeObjectRegistrationRequest.Name, HandleNativeObjectRegistration);
            dispatcher.RegisterMessageHandler(Messages.NativeObjectCallResult.Name, HandleNativeObjectCallResult);

            _javascriptHelper.Register(HandleJavascriptHelperObjectBoundQuery);
        }

        private Task<bool> HandleJavascriptHelperObjectBoundQuery(string objectName)
        {
            return _pendingBoundQueryTasks.GetOrAdd(objectName, _ => new TaskCompletionSource<bool>()).Task;
        }

        private void HandleNativeObjectRegistration(MessageReceivedEventArgs args)
        {
            var browser = args.Browser;
            var context = browser.GetMainFrame().V8Context;

            if (context.Enter())
            {
                try
                {
                    var message = Messages.NativeObjectRegistrationRequest.FromCefMessage(args.Message);

                    var global = context.GetGlobal();
                    var handler = new V8FunctionHandler(message.ObjectName, HandleNativeObjectCall);
                    var attributes = CefV8PropertyAttribute.ReadOnly | CefV8PropertyAttribute.DontEnum | CefV8PropertyAttribute.DontDelete;

                    using (var v8Obj = CefV8Value.CreateObject())
                    {
                        foreach (var methodName in message.MethodsNames)
                        {
                            using (var v8Function = CefV8Value.CreateFunction(methodName, handler))
                            {
                                v8Obj.SetValue(methodName, v8Function, attributes);
                            }
                        }

                        global.SetValue(message.ObjectName, v8Obj);
                    }

                    // notify that the object has been registered, any pending promises on the object will be resolved
                    var taskSource = _pendingBoundQueryTasks.GetOrAdd(message.ObjectName, _ => new TaskCompletionSource<bool>());
                    taskSource.TrySetResult(true);
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

        private PromiseHolder HandleNativeObjectCall(Messages.NativeObjectCallRequest message)
        {
            message.CallId = lastCallId++;

            var promiseHolder = JavascriptHelper.CreatePromise();
            if (!_pendingCalls.TryAdd(message.CallId, promiseHolder))
            {
                throw new InvalidOperationException("Call id already exists");
            }

            var browser = CefV8Context.GetCurrentContext().GetBrowser();
            browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());
            
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

        public void HandleContextReleased(CefV8Context context)
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
}
