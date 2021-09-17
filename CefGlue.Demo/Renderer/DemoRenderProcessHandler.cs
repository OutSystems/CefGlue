namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Xilium.CefGlue;
    //using Xilium.CefGlue.Wrapper;

    class DemoRenderProcessHandler : CefRenderProcessHandler
    {
        internal static bool DumpProcessMessages { get; private set; } = true;

        public DemoRenderProcessHandler()
        {
            //MessageRouter = new CefMessageRouterRendererSide(new CefMessageRouterConfig());
        }

        //internal CefMessageRouterRendererSide MessageRouter { get; private set; }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            //MessageRouter.OnContextCreated(browser, frame, context);

            // MessageRouter.OnContextCreated doesn't capture CefV8Context immediately,
            // so we able to release it immediately in this call.
            context.Dispose();
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            // MessageRouter.OnContextReleased releases captured CefV8Context (if have).
            //MessageRouter.OnContextReleased(browser, frame, context);

            // Release CefV8Context.
            context.Dispose();
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (DumpProcessMessages)
            {
                Debug.Print("Render::OnProcessMessageReceived: SourceProcess={0}", sourceProcess);
                Debug.Print("Message Name={0} IsValid={1} IsReadOnly={2}", message.Name, message.IsValid, message.IsReadOnly);
                var arguments = message.Arguments;
                for (var i = 0; i < arguments.Count; i++)
                {
                    var type = arguments.GetValueType(i);
                    object value;
                    switch (type)
                    {
                        case CefValueType.Null: value = null; break;
                        case CefValueType.String: value = arguments.GetString(i); break;
                        case CefValueType.Int: value = arguments.GetInt(i); break;
                        case CefValueType.Double: value = arguments.GetDouble(i); break;
                        case CefValueType.Bool: value = arguments.GetBool(i); break;
                        default: value = null; break;
                    }

                    Debug.Print("  [{0}] ({1}) = {2}", i, type, value);
                }
            }

            //var handled = MessageRouter.OnProcessMessageReceived(browser, sourceProcess, message);
            //if (handled) return true;

            if (message.Name == "myMessage2") return true;

            // Sending renderer->renderer is not supported.
            //var message2 = CefProcessMessage.Create("myMessage2");
            //frame.SendProcessMessage(CefProcessId.Renderer, message2);
            //Console.WriteLine("Sending myMessage2 to renderer process = {0}");

            var message3 = CefProcessMessage.Create("myMessage3");
            frame.SendProcessMessage(CefProcessId.Browser, message3);
            Debug.Print("Sending myMessage3 to browser process");

            return false;
        }
    }
}
