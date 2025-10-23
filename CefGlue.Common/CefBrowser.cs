using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Common
{
    public class CefBrowserWindow
    {
        public static void Create()
        {
            var logger = new NullLogger("BaseCefBrowser");
            var _adapter = new CommonBrowserAdapter(new object(), "BaseCefBrowser", null, logger, null);
            _adapter.CreateBrowser(800, 800);
        }
    }
}
