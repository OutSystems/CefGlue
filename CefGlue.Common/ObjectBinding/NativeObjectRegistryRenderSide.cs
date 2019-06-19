using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectRegistryRenderSide
    {
        public NativeObjectRegistryRenderSide(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.NativeObjectRegistrationRequest.Name, HandleNativeObjectRegistration);
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
                    var handler = new V8FunctionHandler(message.ObjectName);
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
    }
}
