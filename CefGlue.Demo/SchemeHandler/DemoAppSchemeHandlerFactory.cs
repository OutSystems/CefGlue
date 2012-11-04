namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal sealed class DemoAppSchemeHandlerFactory : CefSchemeHandlerFactory
    {
        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            return new DumpRequestResourceHandler();
        }
    }
}
