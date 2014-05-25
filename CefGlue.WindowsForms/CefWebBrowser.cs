namespace Xilium.CefGlue.WindowsForms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(CefWebBrowser))]
    public class CefWebBrowser : Control
    {
        private bool _handleCreated;

        private CefBrowser _browser;
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

            StartUrl = "about:blank";
        }


        [DefaultValue("about:blank")]
        public string StartUrl { get; set; }

        [Browsable(false)]
        public CefBrowserSettings BrowserSettings { get; set; }

		internal void InvokeIfRequired(Action a)
		{
			if (InvokeRequired)
				Invoke(a);
			else
				a();
		}

        protected virtual CefWebClient CreateWebClient()
        {
            return new CefWebClient(this);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (DesignMode)
            {
                if (!_handleCreated) Paint += PaintInDesignMode;
            }
            else
            {
                var windowInfo = CefWindowInfo.Create();
                windowInfo.SetAsChild(Handle, new CefRectangle { X = 0, Y = 0, Width = Width, Height = Height });

                var client = CreateWebClient();

                var settings = BrowserSettings;
                if (settings == null) settings = new CefBrowserSettings { };

                CefBrowserHost.CreateBrowser(windowInfo, client, settings, StartUrl);
            }

            _handleCreated = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (_browser != null && disposing) // TODO: ugly hack to avoid crashes when CefWebBrowser are Finalized and underlying objects already finalized
            {
                var host = _browser.GetHost();
                if (host != null)
                {
                    host.CloseBrowser();
                    host.Dispose();
                }
                _browser.Dispose();
                _browser = null;
                _browserWindowHandle = IntPtr.Zero;
            }

            base.Dispose(disposing);
        }

    	public event EventHandler BrowserCreated;

        internal protected virtual void OnBrowserAfterCreated(CefBrowser browser)
        {
            _browser = browser;
            _browserWindowHandle = _browser.GetHost().GetWindowHandle();
            ResizeWindow(_browserWindowHandle, Width, Height);

			if (BrowserCreated != null)
				BrowserCreated(this, EventArgs.Empty);
        }

		internal protected virtual void OnTitleChanged(TitleChangedEventArgs e)
        {
            Title = e.Title;

            var handler = TitleChanged;
            if (handler != null) handler(this, e);
        }

        public string Title { get; private set; }

        public event EventHandler<TitleChangedEventArgs> TitleChanged;

        internal protected virtual void OnAddressChanged(AddressChangedEventArgs e)
        {
        	Address = e.Address;

            var handler = AddressChanged;
            if (handler != null) handler(this, e);
        }

        public string Address { get; private set; }

        public event EventHandler<AddressChangedEventArgs> AddressChanged;

		internal protected virtual void OnStatusMessage(StatusMessageEventArgs e)
        {
			var handler = StatusMessage;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<StatusMessageEventArgs> StatusMessage;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_browserWindowHandle != IntPtr.Zero)
            {
                // Ignore size changes when form are minimized.
                var form = TopLevelControl as Form;
                if (form != null && form.WindowState == FormWindowState.Minimized)
                {
                    return;
                }

                ResizeWindow(_browserWindowHandle, Width, Height);
            }
        }

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

		public void InvalidateSize()
		{
			ResizeWindow(_browserWindowHandle, Width, Height);
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

        public CefBrowser Browser { get { return _browser; } }

    	public event EventHandler<ConsoleMessageEventArgs> ConsoleMessage;

		internal protected virtual void OnConsoleMessage(ConsoleMessageEventArgs e)
    	{
			if (ConsoleMessage != null)
				ConsoleMessage(this, e);
			else
				e.Handled = false;
    	}

    	public event EventHandler<LoadingStateChangeEventArgs> LoadingStateChange;

		internal protected virtual void OnLoadingStateChange(LoadingStateChangeEventArgs e)
		{
			if (LoadingStateChange != null)
				LoadingStateChange(this, e);
		}

    	public event EventHandler<TooltipEventArgs> Tooltip;

		internal protected virtual void OnTooltip(TooltipEventArgs e)
		{
			if (Tooltip != null)
				Tooltip(this, e);
			else
				e.Handled = false;
		}

    	public event EventHandler BeforeClose;

		internal protected virtual void OnBeforeClose()
		{
			_browserWindowHandle = IntPtr.Zero;
			if (BeforeClose != null)
				BeforeClose(this, EventArgs.Empty);
		}

    	public event EventHandler<BeforePopupEventArgs> BeforePopup;

		internal protected virtual void OnBeforePopup(BeforePopupEventArgs e)
		{
			if (BeforePopup != null)
				BeforePopup(this, e);
			else
				e.Handled = false;
		}

    	public event EventHandler<LoadEndEventArgs> LoadEnd;

		internal protected virtual void OnLoadEnd(LoadEndEventArgs e)
		{
			if (LoadEnd != null)
				LoadEnd(this, e);
		}

    	public event EventHandler<LoadErrorEventArgs> LoadError;

		internal protected virtual void OnLoadError(LoadErrorEventArgs e)
		{
			if (LoadError != null)
				LoadError(this, e);
		}

    	public event EventHandler<LoadStartEventArgs> LoadStarted;

		internal protected virtual void OnLoadStart(LoadStartEventArgs e)
		{
			if (LoadStarted != null)
				LoadStarted(this, e);
		}

    	public event EventHandler<PluginCrashedEventArgs> PluginCrashed;

		internal protected virtual void OnPluginCrashed(PluginCrashedEventArgs e)
    	{
			if (PluginCrashed != null)
				PluginCrashed(this, e);
    	}

    	public event EventHandler<RenderProcessTerminatedEventArgs> RenderProcessTerminated;

		internal protected virtual void OnRenderProcessTerminated(RenderProcessTerminatedEventArgs e)
		{
			if (RenderProcessTerminated != null)
				RenderProcessTerminated(this, e);
		}
    }
}
