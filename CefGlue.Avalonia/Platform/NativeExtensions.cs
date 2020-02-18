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
            [DllImport("user32.dll")]
            public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
        }
    }
}
