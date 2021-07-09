using System;
using System.Runtime.InteropServices;

namespace Xilium.CefGlue.Avalonia.Platform.MacOS
{
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct CGPoint
    {
        public double X;

        public double Y;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CGSize
    {
        public double Width;

        public double Height;
    }

    internal struct CGRect
    {
        public CGPoint Origin;

        public CGSize Size;
    }

    internal class NSView : IHandleHolder
    {
        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern IntPtr objc_msgSend(IntPtr receiver, IntPtr selector, CGRect arg);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern IntPtr objc_getClass(string className);

        [DllImport("/usr/lib/libobjc.dylib")]
        private static extern void objc_release(IntPtr handle);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern IntPtr sel_registerName(string selectorName);

        private static readonly IntPtr NsViewClass = objc_getClass("NSView");

        private static readonly IntPtr InitSelector = sel_registerName("init");
        private static readonly IntPtr AllocSelector = sel_registerName("alloc");
        private static readonly IntPtr SetFrameSelector = sel_registerName("setFrame:");

        public NSView()
        {
            var nsViewType = objc_msgSend(NsViewClass, AllocSelector);
            Handle = objc_msgSend(nsViewType, InitSelector);
        }

        public NSView(double width, double height) : this()
        {
            SetFrame(new CGRect()
            {
                Size = new CGSize()
                {
                    Width = width,
                    Height = height,
                }
            });
        }

        public IntPtr Handle { get; }

        public void SetFrame(CGRect frame)
        {
            objc_msgSend(Handle, SetFrameSelector, frame);
        }

        public void Dispose()
        {
            objc_release(Handle);
        }
    }
}
