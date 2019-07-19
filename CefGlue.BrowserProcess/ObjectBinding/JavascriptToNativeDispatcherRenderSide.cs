using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xilium.CefGlue.BrowserProcess.Serialization;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal class JavascriptToNativeDispatcherRenderSide
    {
        private readonly IDictionary<int, PromiseHolder> _pendingCalls = new ConcurrentDictionary<int, PromiseHolder>();

        public JavascriptToNativeDispatcherRenderSide(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.NativeObjectRegistrationRequest.Name, HandleNativeObjectRegistration);
            dispatcher.RegisterMessageHandler(Messages.NativeObjectCallResult.Name, HandleNativeObjectCallResult);
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
                    var handler = new V8FunctionHandler(message.ObjectName, _pendingCalls);
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

        private void HandleNativeObjectCallResult(MessageReceivedEventArgs args)
        {
            var message = Messages.NativeObjectCallResult.FromCefMessage(args.Message);
            if (_pendingCalls.TryGetValue(message.CallId, out var promiseHolder))
            {
                try
                {
                    var context = promiseHolder.Context;
                    if (context.Enter())
                    {
                        try
                        {
                            var result = V8ValueSerialization.SerializeCefValue(message.Result);
                            promiseHolder.Resolve.ExecuteFunction(null, new[] { result });
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
                finally
                {
                    _pendingCalls.Remove(message.CallId);
                }
            }
            else
            {
                // TODO
            }
        }

        public void HandleContextReleased(CefV8Context context)
        {
            foreach (var promiseHolderEntry in _pendingCalls.ToArray())
            {
                if (promiseHolderEntry.Value.Context == context)
                {
                    _pendingCalls.Remove(promiseHolderEntry.Key);
                }
            }
        }
    }
}
