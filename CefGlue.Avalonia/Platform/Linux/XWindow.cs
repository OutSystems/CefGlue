using System;
using System.Runtime.InteropServices;
﻿using Avalonia.Controls;
using Avalonia.Platform;

namespace Xilium.CefGlue.Avalonia.Platform.Linux
{
    internal class XWindow : IHandleHolder, IPlatformHandle
    {
        [DllImport("libX11.so.6")]
        private static extern int XDestroyWindow(IntPtr display, IntPtr w);

        [DllImport("libX11.so.6")]
        private static extern IntPtr XOpenDisplay([MarshalAs(UnmanagedType.LPUTF8Str)] string display_name);

        [DllImport("libX11.so.6")]
        private static extern IntPtr XCreateSimpleWindow(IntPtr display, IntPtr parent, int x, int y, uint width, uint height, uint border_width, UIntPtr border, UIntPtr background);

        [DllImport("libX11.so.6")]
        private static extern IntPtr XDefaultRootWindow(IntPtr display);

        [DllImport("libX11.so.6")]

        private static extern int XFlush(IntPtr display);
        private static IntPtr _display;
        private readonly bool _needDispose;

        public XWindow(IntPtr handle) : this(handle, false) { }
        
        private XWindow(IntPtr handle, bool needDispose)
        {
            Handle = handle;
            _needDispose = needDispose;
        }

        public IntPtr Handle { get; }

        public string HandleDescriptor => "XWindow";

        public static IPlatformHandle CreateHostWindow()
        {
            EnsureDisplay();
            var handle = XCreateSimpleWindow(_display, XDefaultRootWindow(_display), 0, 0, 100, 100, 0, (UIntPtr)0, (UIntPtr)0);
            XFlush(_display);
            return new XWindow(handle, true);
        }

        private static void EnsureDisplay()
        {
            if (_display == IntPtr.Zero)
            {
                _display = XOpenDisplay(null);
            }
        }

        public void Dispose()
        {
            var display = _display;
            if (_needDispose && display != IntPtr.Zero)
            {
                XDestroyWindow(display, Handle);
                XFlush(display);
            }
        }
    }
}
