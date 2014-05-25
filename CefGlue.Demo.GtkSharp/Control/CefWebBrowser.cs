namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using Gtk;
    using Xilium.CefGlue.Demo.Browser;

    // TODO: it is have sense to use Box insead of VBox?
    public sealed class CefWebBrowser : VBox
    {
        private WebBrowser _core;
        private IntPtr _browserWindowHandle;

        private int _x;
        private int _y;
        private int _width;
        private int _height;

        private bool _created;

        public CefWebBrowser()
        {
            _core = new WebBrowser(this, new CefBrowserSettings(), "about:blank");
            _core.Created += new EventHandler(BrowserCreated);

            WidgetFlags &= ~(WidgetFlags.DoubleBuffered);
            WidgetFlags |= WidgetFlags.NoWindow;
        }

        protected override bool OnDestroyEvent(Gdk.Event evnt)
        {
            Console.WriteLine("OnDestroyEvent..");
            return base.OnDestroyEvent(evnt);
        }

        protected override void OnDestroyed()
        {
            Console.WriteLine("Destroyed..");

            if (_core != null)
            {
                var browser = _core.CefBrowser;
                var host = browser.GetHost();
                host.CloseBrowser();
                host.Dispose();
                browser.Dispose();
                browser = null;
                _browserWindowHandle = IntPtr.Zero;
            }

            base.OnDestroyed();
        }

        public override void Destroy()
        {
            Console.WriteLine("Destroying..");

            base.Destroy();
        }

        public string StartUrl
        {
            get { return _core.StartUrl; }
            set { _core.StartUrl = value; }
        }

        public WebBrowser WebBrowser { get { return _core; } }

        protected override void OnRealized()
        {
            base.OnRealized();

            ChildVisible = true;

            var windowInfo = CefWindowInfo.Create();
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    var parentHandle = gdk_win32_drawable_get_handle(GdkWindow.Handle);
                    windowInfo.SetAsChild(parentHandle, new CefRectangle(0, 0, 0, 0)); // TODO: set correct  x, y, width, height  to do not waiting OnSizeAllocated event
                    break;

                case CefRuntimePlatform.Linux:
                    Console.WriteLine("REALIZED - RAW = {0}, HANDLE = {1}", Raw, Handle);
                    windowInfo.SetAsChild(Handle, new CefRectangle(0, 0, 0, 0));
                    break;

                case CefRuntimePlatform.MacOSX:
                default:
                    throw new NotSupportedException();
            }

            _core.Create(windowInfo);
        }

        protected override void OnMapped()
        {
            base.OnMapped();

            if (CefRuntime.Platform == CefRuntimePlatform.Windows)
            {
                if (_created)
                {
                    ShowWindow(_browserWindowHandle);
                }
            }
        }

        protected override void OnUnmapped()
        {
            base.OnUnmapped();

            if (CefRuntime.Platform == CefRuntimePlatform.Windows)
            {
                if (_created)
                {
                    HideWindow(_browserWindowHandle);
                }
            }
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);

            // FIXME: fix formatting
            if (CefRuntime.Platform == CefRuntimePlatform.Windows)
            {
            if (_x != allocation.X
                || _y != allocation.Y
                || _width != allocation.Width
                || _height != allocation.Height
                )
            {
                _x = allocation.X;
                _y = allocation.Y;
                _width = allocation.Width;
                _height = allocation.Height;
                Console.WriteLine("OnSizeAllocated {0}x{1}", _width, _height);
                if (_created)
                {
                    // TODO: use one ResizeWindow method, remove show/hide window methods
                    if (IsMapped) ShowWindow(_browserWindowHandle); else HideWindow(_browserWindowHandle);
                    ResizeWindow(_browserWindowHandle, _x, _y, _width, _height);
                }
            }
            }
        }

        private void BrowserCreated(object sender, EventArgs e)
        {
            _browserWindowHandle = _core.CefBrowser.GetHost().GetWindowHandle();
            _created = true;

            if (CefRuntime.Platform == CefRuntimePlatform.Windows)
            {
                if (IsMapped) ShowWindow(_browserWindowHandle); else HideWindow(_browserWindowHandle);
                ResizeWindow(_browserWindowHandle, Allocation.X, Allocation.Y, Allocation.Width, Allocation.Height);
            }
        }

        [DllImport("libgdk-win32-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr gdk_win32_drawable_get_handle(IntPtr raw);

        private static void ResizeWindow(IntPtr handle, int x, int y, int width, int height)
        {
            if (handle != IntPtr.Zero)
            {
                NativeMethods.SetWindowPos(handle, IntPtr.Zero,
                    x, y, width, height,
                    SetWindowPosFlags.NoZOrder
                    );
            }
        }

        private static void ShowWindow(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                NativeMethods.SetWindowPos(handle, IntPtr.Zero,
                    0, 0, 0, 0,
                    SetWindowPosFlags.ShowWindow | SetWindowPosFlags.NoMove | SetWindowPosFlags.NoSize | SetWindowPosFlags.NoZOrder
                    );
            }
        }

        private static void HideWindow(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                NativeMethods.SetWindowPos(handle, IntPtr.Zero,
                    0, 0, 0, 0,
                    SetWindowPosFlags.HideWindow | SetWindowPosFlags.NoMove | SetWindowPosFlags.NoSize | SetWindowPosFlags.NoZOrder
                    );
            }
        }
    }
}
