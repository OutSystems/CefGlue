using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Xilium.CefGlue.WPF.Platform
{
    internal class ExtendedWpfNativeControlHost : HwndHost
    {
        private readonly IntPtr _browserHandle;

        public ExtendedWpfNativeControlHost(IntPtr browserHandle)
        {
            _browserHandle = browserHandle;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            return new HandleRef(this, _browserHandle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            // do nothing
        }
    }
}
