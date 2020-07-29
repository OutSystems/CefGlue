
namespace Xilium.CefGlue.Demo.Avalonia
{
    class CustomSchemeHandler : CefSchemeHandlerFactory
    {
        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}