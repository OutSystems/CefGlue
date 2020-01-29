using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Platform;

namespace CefGlue.Tests.Helpers
{
    public class GenericCefBrowser : BaseCefBrowser
    {

        public GenericCefBrowser()
        {
            CreateOrUpdateBrowser(0, 0, 100, 100);
            UnhandledException += GenericCefBrowser_UnhandledException;
        }

        private void GenericCefBrowser_UnhandledException(object sender, Xilium.CefGlue.Common.Events.AsyncUnhandledExceptionEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        internal override IControl CreateControl()
        {
            return new DummyControl();
        }

        internal override IPopup CreatePopup()
        {
            return new DummyPopup();
        }
    }
}
