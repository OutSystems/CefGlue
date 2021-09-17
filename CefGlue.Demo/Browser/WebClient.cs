namespace Xilium.CefGlue.Demo.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;

    internal sealed class WebClient : CefClient
    {
        internal static bool DumpProcessMessages { get; set; }

        private readonly WebBrowser _core;
        private readonly WebLifeSpanHandler _lifeSpanHandler;
        private readonly WebFrameHandler _frameHandler;
        private readonly WebDisplayHandler _displayHandler;
        private readonly WebLoadHandler _loadHandler;
        private readonly WebKeyboardHandler _keyboardHandler;

        public WebClient(WebBrowser core)
        {
            _core = core;
            _lifeSpanHandler = new WebLifeSpanHandler(_core);
            _frameHandler = new WebFrameHandler(_core);
            _displayHandler = new WebDisplayHandler(_core);
            _loadHandler = new WebLoadHandler(_core);
            _keyboardHandler = new WebKeyboardHandler(_core);
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefFrameHandler GetFrameHandler() => _frameHandler;

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _displayHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }

        protected override CefKeyboardHandler GetKeyboardHandler() => _keyboardHandler;

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (DumpProcessMessages)
            {
                Console.WriteLine("Client::OnProcessMessageReceived: SourceProcess={0}", sourceProcess);
                Console.WriteLine("Message Name={0} IsValid={1} IsReadOnly={2}", message.Name, message.IsValid, message.IsReadOnly);
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

                    Console.WriteLine("  [{0}] ({1}) = {2}", i, type, value);
                }
            }

            //var handled = DemoApp.BrowserMessageRouter.OnProcessMessageReceived(browser, sourceProcess, message);
            //if (handled) return true;

            if (message.Name == "myMessage2" || message.Name == "myMessage3") return true;

            return false;
        }
    }
}
