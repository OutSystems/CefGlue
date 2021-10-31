namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WebBrowser = Xilium.CefGlue.Demo.Browser.WebBrowser;
    using System.Windows.Forms;
    using Xilium.CefGlue.Platform.Windows;
    using System.Runtime.InteropServices;

    // There is old / incomplete code. Don't rely on it.
    public class CefWebView
    {
        private bool _handleCreated;
        private WebBrowser _core;
        private IntPtr _browserWindowHandle;

        public CefWebView(string url, bool transparent = false)
        {
            _core = new WebBrowser(this, new CefBrowserSettings(), url ?? "about:blank");
            _core.Created += new EventHandler(BrowserCreated);

            var windowInfo = CefWindowInfo.Create();
            windowInfo.SetAsPopup(IntPtr.Zero, null);
            windowInfo.Name = url;
            if (transparent)
            {
                windowInfo.Bounds = new CefRectangle(int.MinValue, int.MinValue, width: 500, height: 500);
                //windowInfo.Style = WindowStyle.WS_POPUP | WindowStyle.WS_SIZEFRAME | WindowStyle.WS_VISIBLE;
                windowInfo.StyleEx = WindowStyleEx.WS_EX_COMPOSITED;
            }

            _core.Create(windowInfo);
        }

        private void BrowserCreated(object sender, EventArgs e)
        {
            var handle = _core.CefBrowser.GetHost().GetWindowHandle();

            //MARGINS mgMarInset = new MARGINS { leftWidth = -1, rightWidth = -1, topHeight = -1, bottomHeight = -1 };
            //DwmExtendFrameIntoClientArea(handle, ref mgMarInset);
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
    }
}
