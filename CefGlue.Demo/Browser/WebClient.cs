namespace Xilium.CefGlue.Demo.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;

    internal sealed class WebClient : CefClient
    {
        private readonly WebBrowser _core;
        private readonly WebLifeSpanHandler _lifeSpanHandler;
        private readonly WebDisplayHandler _displayHandler;

        public WebClient(WebBrowser core)
        {
            _core = core;
            _lifeSpanHandler = new WebLifeSpanHandler(_core);
            _displayHandler = new WebDisplayHandler(_core);
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _displayHandler;
        }


    }
}
