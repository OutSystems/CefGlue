using CefGlue.Tests.Helpers;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Handlers;

namespace CefGlue.Tests.CustomSchemes
{
    class CustomSchemeHandlerFactory : CefSchemeHandlerFactory
    {
        public const string SchemeName = "test";

        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            return new DefaultResourceHandler()
            {
                Response = StreamHelper.GetStream("test")
            };
        }
    }
}