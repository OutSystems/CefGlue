namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Xilium.CefGlue;

    public partial class MainForm : Form
    {
        private CefBrowser _browser;
        private IntPtr _browserWindowHandle;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            Debug.Assert(IsHandleCreated);

            var windowInfo = new CefWindowInfo();
            windowInfo.Parent = Handle;

            var client = new DemoClient(this);

            var settings = new CefBrowserSettings
                {
                    AuthorAndUserStylesDisabled = false,
                };

            var url = "http://www.google.com";

            CefBrowserHost.CreateBrowser(windowInfo, client, settings, url);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_browser != null)
            {
                _browser.GetHost().ParentWindowWillClose();
            }

            base.OnClosing(e);
        }

        private void OnBrowserCreated(CefBrowser browser)
        {
            _browser = browser;
            _browserWindowHandle = _browser.GetHost().GetWindowHandle();
            ResizeWindow(_browserWindowHandle, this.ClientSize.Width, this.ClientSize.Height);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResizeWindow(_browserWindowHandle, this.ClientSize.Width, this.ClientSize.Height);
        }

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

        internal sealed class DemoClient : CefClient
        {
            private readonly CefLifeSpanHandler _lifeSpanHandler;

            public DemoClient(MainForm form)
            {
                _lifeSpanHandler = new DemoLifeSpanHandler(form);
            }

            protected override CefLifeSpanHandler GetLifeSpanHandler()
            {
                return _lifeSpanHandler;
            }
        }

        internal sealed class DemoLifeSpanHandler : CefLifeSpanHandler
        {
            private readonly MainForm _form;

            public DemoLifeSpanHandler(MainForm form)
            {
                _form = form;
            }

            protected override void OnAfterCreated(CefBrowser browser)
            {
                if (_form != null && !_form.IsDisposed)
                {
                    _form.OnBrowserCreated(browser);
                }

                base.OnAfterCreated(browser);
            }
        }
    }
}
