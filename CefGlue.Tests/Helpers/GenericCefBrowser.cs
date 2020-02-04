using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Platform;

namespace CefGlue.Tests.Helpers
{
    public class GenericCefBrowser : BaseCefBrowser
    {
        private DummyControl _control;

        public GenericCefBrowser()
        {
            UnhandledException += GenericCefBrowser_UnhandledException;

            _control.TriggerInitialization();
        }

        private void GenericCefBrowser_UnhandledException(object sender, Xilium.CefGlue.Common.Events.AsyncUnhandledExceptionEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        internal override IOffScreenControlHost CreateOffScreenControlHost()
        {
            _control = new DummyControl();
            return _control;
        }

        internal override IOffScreenPopupHost CreatePopupHost()
        {
            return new DummyPopup();
        }

        internal override IControl CreateControl()
        {
            _control = new DummyControl();
            return _control;
        }
    }
}
