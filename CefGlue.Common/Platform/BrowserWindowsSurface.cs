using System;
using System.Runtime.InteropServices;

namespace Xilium.CefGlue.Common.Platform
{
    internal class BrowserWindowsSurface : BaseBrowserSurface
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        private static extern int GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        private static IntPtr GetWindowLong(IntPtr hwnd, int nIndex)
        {
            if (IntPtr.Size == 8)
            {
                return GetWindowLongPtr64(hwnd, nIndex);
            }
            return new IntPtr(GetWindowLongPtr32(hwnd, nIndex));
        }

        private static IntPtr SetWindowLong(IntPtr hwnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hwnd, nIndex, dwNewLong);
            }
            return new IntPtr(SetWindowLongPtr32(hwnd, nIndex, dwNewLong.ToInt32()));
        }

        private static bool ModifyStyle(IntPtr handle, uint removeStyle, uint addStyle)
        {
            const int GWL_STYLE = -16;
            var dwStyle = (uint)GetWindowLong(handle, GWL_STYLE).ToInt32();
            var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
            if (dwStyle == dwNewStyle)
            {
                return false;
            }

            SetWindowLong(handle, GWL_STYLE, new IntPtr((int)dwNewStyle));
            return true;
        }

        private CefRectangle _viewRect;

        public BrowserWindowsSurface(IntPtr windowHandle)
        {
            // prevent parent window from drawing over browser
            //const uint WS_CLIPCHILDREN = 0x02000000;
            //ModifyStyle(windowHandle, 0, WS_CLIPCHILDREN);
        }

        public override void Hide()
        {
            //const int SW_HIDE = 0;

            //if (_browserHost != null)
            //{
            //    ShowWindow(_browserHost.GetWindowHandle(), SW_HIDE);
            //}
        }

        public override bool MoveAndResize(int x, int y, int width, int height)
        {
            _viewRect = new CefRectangle(x, y, width, height);
            //if (_browserHost != null)
            //{
            //    MoveWindow(_browserHost.GetWindowHandle(), x, y, width, height, false);
            //}

            return true;
        }

        public override void Show()
        {
            //const int SW_SHOW = 5;
            //if (_browserHost != null)
            //{
            //    ShowWindow(_browserHost.GetWindowHandle(), SW_SHOW);
            //}
        }

        public override void SetBrowserHost(CefBrowserHost browserHost)
        {
            base.SetBrowserHost(browserHost);
            //if (_viewRect.Width > 0 && _viewRect.Height > 0)
            //{
            //    MoveWindow(_browserHost.GetWindowHandle(), _viewRect.X, _viewRect.Y, _viewRect.Width, _viewRect.Height, false);
            //}

        }

        public override CefRectangle GetViewRect() => _viewRect;
    }
}
