using Xilium.CefGlue.Common;

namespace CefGlue.Tests.Helpers
{
    public class GenericCefBrowser : BaseCefBrowser
    {

        public GenericCefBrowser()
        {
            CreateOrUpdateBrowser(100, 100);
            UnhandledException += GenericCefBrowser_UnhandledException;
        }

        private void GenericCefBrowser_UnhandledException(object sender, Xilium.CefGlue.Common.Events.AsyncUnhandledExceptionEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        internal override CommonBrowserAdapter CreateAdapter()
        {
            return new CommonBrowserAdapter(this, nameof(GenericCefBrowser), new DummyControl(), new DummyPopup(), new DummyLogger());
        }
    }
}
