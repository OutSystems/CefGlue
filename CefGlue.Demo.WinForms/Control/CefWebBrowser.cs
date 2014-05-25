namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;
    using Xilium.CefGlue.Demo.Browser;
    using WebBrowser = Xilium.CefGlue.Demo.Browser.WebBrowser;

    public sealed class CefWebBrowser : Control
    {
        private bool _handleCreated;

        private WebBrowser _core;
        private IntPtr _browserWindowHandle;

        public CefWebBrowser()
        {
            SetStyle(
                ControlStyles.ContainerControl
                | ControlStyles.ResizeRedraw
                | ControlStyles.FixedWidth
                | ControlStyles.FixedHeight
                | ControlStyles.StandardClick
                | ControlStyles.UserMouse
                | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.StandardDoubleClick
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.CacheText
                | ControlStyles.EnableNotifyMessage
                | ControlStyles.DoubleBuffer
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.UseTextForAccessibility
                | ControlStyles.Opaque,
                false);

            SetStyle(
                ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.Selectable,
                true);

            var settings = new CefBrowserSettings();
            // settings.ImageLoading = CefState.Disabled;
            // settings.AcceleratedCompositing = CefState.Disabled;

            _core = new WebBrowser(this, settings, "about:blank");
            _core.Created += new EventHandler(BrowserCreated);
        }

        public string StartUrl
        {
            get { return _core.StartUrl; }
            set { _core.StartUrl = value; }
        }

        public WebBrowser WebBrowser { get { return _core; } }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (DesignMode)
            {
                // if (!_handleCreated) Paint += PaintInDesignMode;
            }
            else
            {
                var windowInfo = CefWindowInfo.Create();
                windowInfo.SetAsChild(Handle, new CefRectangle { X = 0, Y = 0, Width = Width, Height = Height });

                _core.Create(windowInfo);
            }

            _handleCreated = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (_core != null && disposing)
            {
                _core.Close();
            }

            _core = null;
            _browserWindowHandle = IntPtr.Zero;

            base.Dispose(disposing);
        }

        internal void BrowserCreated(object sender, EventArgs e)
        {
            // _browser = browser;
            _browserWindowHandle = _core.CefBrowser.GetHost().GetWindowHandle();
            ResizeWindow(_browserWindowHandle, Width, Height);
        }

        /*
        internal void OnTitleChanged(string title)
        {
            Title = title;

            var handler = TitleChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public string Title { get; private set; }

        public event EventHandler TitleChanged;

        internal void OnAddressChanged(string address)
        {
            Address = address;

            var handler = AddressChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public string Address { get; private set; }

        public event EventHandler AddressChanged;

        internal void OnStatusMessage(string value)
        {
            var handler = StatusMessage;
            if (handler != null) handler(this, new StatusMessageEventArgs(value));
        }

        public event EventHandler<StatusMessageEventArgs> StatusMessage;
        */

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var form = TopLevelControl as Form;
            if (form != null && form.WindowState != FormWindowState.Minimized)
            {
                ResizeWindow(_browserWindowHandle, Width, Height);
            }
        }

        /*
        private void PaintInDesignMode(object sender, PaintEventArgs e)
        {
            var width = this.Width;
            var height = this.Height;
            if (width > 1 && height > 1)
            {
                var brush = new SolidBrush(this.ForeColor);
                var pen = new Pen(this.ForeColor);
                pen.DashStyle = DashStyle.Dash;

                e.Graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);

                var fontHeight = (int)(this.Font.GetHeight(e.Graphics) * 1.25);

                var x = 3;
                var y = 3;

                e.Graphics.DrawString("CefWebBrowser", Font, brush, x, y + (0 * fontHeight));
                e.Graphics.DrawString(string.Format("StartUrl: {0}", StartUrl), Font, brush, x, y + (1 * fontHeight));

                brush.Dispose();
                pen.Dispose();
            }
        }
        */

        private static void ResizeWindow(IntPtr handle, int width, int height)
        {
            if (handle != IntPtr.Zero)
            {
                NativeMethods.SetWindowPos(handle, IntPtr.Zero,
                    0, 0, width, height,
                    SetWindowPosFlags.NoMove | SetWindowPosFlags.NoZOrder
                    );
            }
        }

        // public WebBrowserCore Browser { get { return _browser; } }
    }
}
