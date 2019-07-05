using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.WPF
{
    public class WpfCefBrowser : ContentControl, IDisposable
    {
        private readonly ILogger _logger;
        private readonly WpfBrowserAdapter _adapter;

        public WpfCefBrowser() : this(new NLogLogger(nameof(WpfCefBrowser))) {
        }

        private WpfCefBrowser(ILogger logger) {
            if (logger == null) {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
            _adapter = new WpfBrowserAdapter(this, logger);
        }

        #region Disposable

        ~WpfCefBrowser() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            _adapter.Dispose(disposing);
        }

        #endregion

        public event LoadStartEventHandler LoadStart { add => _adapter.LoadStart += value; remove => _adapter.LoadStart -= value; }
        public event LoadEndEventHandler LoadEnd { add => _adapter.LoadEnd += value; remove => _adapter.LoadEnd -= value; }
        public event LoadingStateChangeEventHandler LoadingStateChange { add => _adapter.LoadingStateChange += value; remove => _adapter.LoadingStateChange -= value; }
        public event LoadErrorEventHandler LoadError { add => _adapter.LoadError += value; remove => _adapter.LoadError -= value; }

        public string StartUrl { get => _adapter.StartUrl; set => _adapter.StartUrl = value; }

        public bool AllowsTransparency { get => _adapter.AllowsTransparency; set => _adapter.AllowsTransparency = value; }

        public void NavigateTo(string url) {
            _adapter.NavigateTo(url);
        }

        public void LoadString(string content, string url) {
            _adapter.LoadString(content, url);
        }

        public bool CanGoBack() {
            return _adapter.CanGoBack();
        }

        public void GoBack() {
            _adapter.GoBack();
        }

        public bool CanGoForward() {
            return _adapter.CanGoForward();
        }

        public void GoForward() {
            _adapter.GoForward();
        }

        public void Refresh() {
            _adapter.Refresh();
        }

        public void ExecuteJavaScript(string code, string url = null, int line = 1) {
            _adapter.ExecuteJavaScript(code, url ?? "about:blank", line);
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url = null, int line = 1) {
            return _adapter.EvaluateJavaScript<T>(code, url ?? "about:blank", line);
        }

        public void ShowDeveloperTools() {
            _adapter.ShowDeveloperTools();
        }

        public void RegisterJavascriptObject(object targetObject, string name) {
            _adapter.RegisterJavascriptObject(targetObject, name);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //this.browserPageD3dImage = new D3DImage();

            _browserPageImage = new Image()
            {
                Focusable = false,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.None
            };

            this.Content = _browserPageImage;

            // TODO wpf
            //    protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
            //{
            //    base.OnTemplateApplied(e);
            //    _adapter.BrowserImage = e.NameScope.Find<Image>("PART_Image");
            //}
        }


        

        protected override Size ArrangeOverride(Size arrangeBounds) {
            var size = base.ArrangeOverride(arrangeBounds);

            if (_adapter.BrowserImage != null) {
                _adapter.CreateOrUpdateBrowser((int)size.Width, (int)size.Height);
            }

            return size;

            // TODO WPF
            //protected override Size ArrangeOverride(Size arrangeBounds)
            //{
            //    var size = base.ArrangeOverride(arrangeBounds);

            //    if (_browserPageImage != null)
            //    {
            //        var newWidth = (int)size.Width;
            //        var newHeight = (int)size.Height;

            //        _logger.Debug("BrowserResize. Old H{0}xW{1}; New H{2}xW{3}.", _browserHeight, _browserWidth, newHeight, newWidth);

            //        if (newWidth > 0 && newHeight > 0)
            //        {
            //            if (!_created)
            //            {
            //                AttachEventHandlers(this); // TODO: ?

            //                // Create the bitmap that holds the rendered website bitmap
            //                _browserWidth = newWidth;
            //                _browserHeight = newHeight;
            //                _browserSizeChanged = true;

            //                // Find the window that's hosting us
            //                Window parentWnd = FindParentOfType<Window>(this);
            //                if (parentWnd != null)
            //                {
            //                    IntPtr hParentWnd = new WindowInteropHelper(parentWnd).Handle;

            //                    var windowInfo = CefWindowInfo.Create();
            //                    windowInfo.SetAsWindowless(hParentWnd, AllowsTransparency);

            //                    var settings = new CefBrowserSettings();
            //                    _cefClient = new WpfCefClient(this);

            //                    // This is the first time the window is being rendered, so create it.
            //                    CefBrowserHost.CreateBrowser(windowInfo, _cefClient, settings, !string.IsNullOrEmpty(StartUrl) ? StartUrl : "about:blank");

            //                    _created = true;
            //                }
            //            }
            //            else
            //            {
            //                // Only update the bitmap if the size has changed
            //                if (_browserPageBitmap == null || (_browserPageBitmap.Width != newWidth || _browserPageBitmap.Height != newHeight))
            //                {
            //                    _browserWidth = newWidth;
            //                    _browserHeight = newHeight;
            //                    _browserSizeChanged = true;

            //                    // If the window has already been created, just resize it
            //                    if (_browserHost != null)
            //                    {
            //                        _logger.Trace("CefBrowserHost::WasResized to {0}x{1}.", newWidth, newHeight);
            //                        _browserHost.WasResized();
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    return size;
            //}
        }










        private static readonly Key[] HandledKeys =
        {
            Key.Tab, Key.Home, Key.End, Key.Left, Key.Right, Key.Up, Key.Down
        };

        private bool _disposed;
        private bool _created;

        private Image _browserPageImage;
        private WriteableBitmap _browserPageBitmap;

        private int _browserWidth;
        private int _browserHeight;
        private bool _browserSizeChanged;

        private CefBrowser _browser;
        private CefBrowserHost _browserHost;
        private WpfCefClient _cefClient;

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupImageBitmap;

        private ToolTip _tooltip;
        private DispatcherTimer _tooltipTimer;

        Dispatcher _mainUiDispatcher;

        private readonly ILogger _logger;

        public WpfCefBrowser() : this(new NLogLogger("WpfCefBrowser"))
        {
        }

        public WpfCefBrowser(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;

            StartUrl = "about:blank";

            _popup = CreatePopup();

            _tooltip = new ToolTip();
            _tooltip.StaysOpen = true;
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Closed += TooltipOnClosed;

            _tooltipTimer = new DispatcherTimer();
            _tooltipTimer.Interval = TimeSpan.FromSeconds(0.5);

            KeyboardNavigation.SetAcceptsReturn(this, true);
            _mainUiDispatcher = Dispatcher.CurrentDispatcher;
        }

        #region Disposable

        ~WpfCefBrowser()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_tooltipTimer != null)
                {
                    _tooltipTimer.Stop();
                }

                if (_browserPageImage != null)
                {
                    _browserPageImage.Source = null;
                    _browserPageImage = null;
                }

                if (_browserPageBitmap != null)
                {
                    _browserPageBitmap = null;
                }

                // 					if (this.browserPageD3dImage != null)
                // 						this.browserPageD3dImage = null;

                // TODO: What's the right way of disposing the browser instance?
                if (_browserHost != null)
                {
                    _browserHost.CloseBrowser();
                    _browserHost = null;
                }

                if (_browser != null)
                {
                    _browser.Dispose();
                    _browser = null;
                }
            }

            _disposed = true;
        }

        #endregion



        internal void HandleViewPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            // When browser size changed - we just skip frame updating.
            // This is dirty precheck to do not do Invoke whenever is possible.
            if (_browserSizeChanged && (width != _browserWidth || height != _browserHeight)) return;

            _mainUiDispatcher.Invoke(DispatcherPriority.Render, new Action(delegate
            {
                // Actual browser size changed check.
                if (_browserSizeChanged && (width != _browserWidth || height != _browserHeight)) return;

                try
                {
                    if (_browserSizeChanged)
                    {
                        _browserPageBitmap = new WriteableBitmap((int)_browserWidth, (int)_browserHeight, 96, 96, AllowsTransparency ? PixelFormats.Bgra32 : PixelFormats.Bgr32, null);
                        _browserPageImage.Source = _browserPageBitmap;

                        _browserSizeChanged = false;
                    }

                    if (_browserPageBitmap != null)
                    {
                        DoRenderBrowser(_browserPageBitmap, width, height, dirtyRects, buffer);
                    }

                }
                catch (Exception ex)
                {
                    _logger.ErrorException("WpfCefBrowser: Caught exception in HandleViewPaint()", ex);
                }
            }));
        }

        internal void HandlePopupPaint(int width, int height, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            if (width == 0 || height == 0)
            {
                return;
            }

            _mainUiDispatcher.Invoke(
                DispatcherPriority.Render,
                new Action(
                    () =>
                    {
                        int stride = width * 4;
                        int sourceBufferSize = stride * height;

                        _logger.Debug("RenderPopup() Bitmap H{0}xW{1}, Browser H{2}xW{3}", _popupImageBitmap.Height, _popupImageBitmap.Width, width, height);


                        foreach (CefRectangle dirtyRect in dirtyRects)
                        {
                            _logger.Debug(
                                string.Format(
                                    "Dirty rect [{0},{1},{2},{3}]",
                                    dirtyRect.X,
                                    dirtyRect.Y,
                                    dirtyRect.Width,
                                    dirtyRect.Height));

                            if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                            {
                                continue;
                            }

                            int adjustedWidth = dirtyRect.Width;

                            int adjustedHeight = dirtyRect.Height;

                            Int32Rect sourceRect = new Int32Rect(dirtyRect.X, dirtyRect.Y, adjustedWidth, adjustedHeight);

                            _popupImageBitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, dirtyRect.X, dirtyRect.Y);
                        }
                    }));
        }

        private void DoRenderBrowser(WriteableBitmap bitmap, int browserWidth, int browserHeight, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            int stride = browserWidth * 4;
            int sourceBufferSize = stride * browserHeight;

            _logger.Debug("DoRenderBrowser() Bitmap H{0}xW{1}, Browser H{2}xW{3}", bitmap.Height, bitmap.Width, browserHeight, browserWidth);

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            foreach (CefRectangle dirtyRect in dirtyRects)
            {
                _logger.Debug(string.Format("Dirty rect [{0},{1},{2},{3}]", dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height));

                if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                {
                    continue;
                }

                // If the window has been resized, make sure we never try to render too much
                int adjustedWidth = (int)dirtyRect.Width;
                //if (dirtyRect.X + dirtyRect.Width > (int) bitmap.Width)
                //{
                //    adjustedWidth = (int) bitmap.Width - (int) dirtyRect.X;
                //}

                int adjustedHeight = (int)dirtyRect.Height;
                //if (dirtyRect.Y + dirtyRect.Height > (int) bitmap.Height)
                //{
                //    adjustedHeight = (int) bitmap.Height - (int) dirtyRect.Y;
                //}

                // Update the dirty region
                Int32Rect sourceRect = new Int32Rect((int)dirtyRect.X, (int)dirtyRect.Y, adjustedWidth, adjustedHeight);
                bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, (int)dirtyRect.X, (int)dirtyRect.Y);

                // 			int adjustedWidth = browserWidth;
                // 			if (browserWidth > (int)bitmap.Width)
                // 				adjustedWidth = (int)bitmap.Width;
                // 
                // 			int adjustedHeight = browserHeight;
                // 			if (browserHeight > (int)bitmap.Height)
                // 				adjustedHeight = (int)bitmap.Height;
                // 
                // 			int sourceBufferSize = browserWidth * browserHeight * 4;
                // 			int stride = browserWidth * 4;
                // 
                // 			Int32Rect sourceRect = new Int32Rect(0, 0, adjustedWidth, adjustedHeight);
                // 			bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, 0, 0);
            }
        }

        internal void OnPopupShow(bool show)
        {
            if (_popup == null)
            {
                return;
            }

            _mainUiDispatcher.Invoke(new Action(() => _popup.IsOpen = show));
        }

    }
}
