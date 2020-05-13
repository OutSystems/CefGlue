using System;
using System.Runtime.InteropServices;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal static class NativeExtensions
    {
        public static class OSX
        {
            [DllImport("/usr/lib/libobjc.dylib")]
            public static extern void objc_retain(IntPtr handle);

            [DllImport("/usr/lib/libobjc.dylib")]
            public static extern void objc_release(IntPtr handle);

            [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
            public static extern IntPtr objc_getClass(string className);

            [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
            public static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

            [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
            public static extern IntPtr sel_registerName(string selectorName);

        }

        public static class Windows
        {
            public enum GWL
            {
                STYLE = (-16),
                EXSTYLE = (-20),
            }

            public enum WS : uint
            {
                CLIPCHILDREN = 0x02000000,
            }

            [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
            public static extern bool DestroyWindow(IntPtr hwnd);

            [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
            private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, GWL nIndex, IntPtr dwNewLong);

            [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
            private static extern int SetWindowLongPtr32(IntPtr hWnd, GWL nIndex, int dwNewLong);

            public static IntPtr SetWindowLong(IntPtr hwnd, GWL nIndex, WS dwNew)
            {
                if (8 == IntPtr.Size)
                {
                    return SetWindowLongPtr64(hwnd, nIndex, new IntPtr((long) dwNew));
                }
                return new IntPtr(SetWindowLongPtr32(hwnd, nIndex, (int) dwNew));
            }
        }
    }
}
