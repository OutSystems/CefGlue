using System;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Platform.Windows;

namespace Xilium.CefGlue.Common
{
    public class CommonBrowserAdapter : ICefBrowserHost, IDisposable
    {
        private string _initialUrl;
        private string _title;
        private string _tooltip;
        private CefBrowser _browser;
        private object _disposeLock = new object();

        public static void CreateBrowser()
        {
            var browser = new CommonBrowserAdapter();
            browser.CreateBrowser(1000, 700);
        }
        public CommonBrowserAdapter()
        {
        }

        ~CommonBrowserAdapter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            var disposeLock = _disposeLock;
            if (disposeLock == null)
            {
                return; // already disposed
            }

            lock (disposeLock)
            {
                if (_disposeLock == null)
                {
                    return; // already disposed
                }

                _disposeLock = null;
            }

            var browserHost = BrowserHost;
            if (browserHost != null)
            {
                if (disposing)
                {
                    browserHost.CloseBrowser(true);
                }
            }

            if (disposing)
            {
                InnerDispose();
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void InnerDispose() { }

        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;
        public event LoadingStateChangeEventHandler LoadingStateChange;
        public event LoadErrorEventHandler LoadError;

        public event Action Initialized;
        public event AddressChangedEventHandler AddressChanged;
        public event TitleChangedEventHandler TitleChanged;
        public event ConsoleMessageEventHandler ConsoleMessage;
        public event StatusMessageEventHandler StatusMessage;

        public event JavascriptContextLifetimeEventHandler JavascriptContextCreated;
        public event JavascriptContextLifetimeEventHandler JavascriptContextReleased;
        public event JavascriptUncaughtExceptionEventHandler JavascriptUncaughtException;

        public event AsyncUnhandledExceptionEventHandler UnhandledException;

        public CefRequestContext RequestContext { get; }

        public string Address { get => _browser?.GetMainFrame().Url ?? _initialUrl; set => NavigateTo(value); }

        #region Cef Handlers

        public ContextMenuHandler ContextMenuHandler { get; set; }
        public DialogHandler DialogHandler { get; set; }
        public DownloadHandler DownloadHandler { get; set; }
        public DragHandler DragHandler { get; set; }
        public FindHandler FindHandler { get; set; }
        public FocusHandler FocusHandler { get; set; }
        public KeyboardHandler KeyboardHandler { get; set; }
        public RequestHandler RequestHandler { get; set; }
        public LifeSpanHandler LifeSpanHandler { get; set; }
        public DisplayHandler DisplayHandler { get; set; }
        public RenderHandler RenderHandler { get; set; }
        public JSDialogHandler JSDialogHandler { get; set; }

        #endregion


        protected CefBrowserHost BrowserHost { get; private set; }

        protected bool IsBrowserCreated { get; private set; }

        public bool IsInitialized => _browser != null;

        public bool IsLoading => _browser?.IsLoading ?? false;

        public string Title => _title;

        public double ZoomLevel
        {
            get => BrowserHost?.GetZoomLevel() ?? 0;
            set => BrowserHost?.SetZoomLevel(value);
        }

        public bool IsJavascriptEngineInitialized { get; private set; }

        public CefBrowserSettings Settings { get; } = new CefBrowserSettings();

        public CefBrowser Browser => _browser;

        public double DefaultZoomLevel => BrowserHost?.GetDefaultZoomLevel() ?? 0.0;

        public bool IsFullscreen => BrowserHost?.IsFullscreen() ?? false;

        private void NavigateTo(string url)
        {
            // Remove leading whitespace from the URL
            url = url.TrimStart();

            // to play safe, load url must be called after OnBrowserCreated(CefBrowser) which runs on CefThreadId.UI, 
            // otherwise the navigation will be aborted
            ActionTask.Run(() =>
            {
                if (IsInitialized)
                {
                    _browser?.GetMainFrame()?.LoadUrl(url);
                }
                else
                {
                    _initialUrl = url;
                }
            });
        }

        public bool CanGoBack()
        {
            return _browser?.CanGoBack ?? false;
        }

        public void GoBack()
        {
            _browser?.GoBack();
        }

        public bool CanGoForward()
        {
            return _browser?.CanGoForward ?? false;
        }

        public void GoForward()
        {
            _browser?.GoForward();
        }

        public void Reload(bool ignoreCache)
        {
            if (ignoreCache)
            {
                _browser?.ReloadIgnoreCache();
            }
            else
            {
                _browser?.Reload();
            }
        }

        public void SendKeyEvent(CefKeyEvent keyEvent)
        {
            BrowserHost?.SendKeyEvent(keyEvent);
        }

        public void ExecuteJavaScript(string code, string url, int line)
        {
            _browser?.GetMainFrame().ExecuteJavaScript(code, url, line);
        }

        public bool CreateBrowser(int width, int height)
        {
            if (IsBrowserCreated || width < 0 || height < 0)
            {
                return false;
            }

            IsBrowserCreated = true;

            var windowInfo = CefWindowInfo.Create();
            SetupBrowserView(windowInfo, width, height);

            using (var extraInfo = CefDictionaryValue.Create())
            {
                // This is the first time the window is being rendered, so create it.
                CefBrowserHost.CreateBrowser(windowInfo, Settings, "", extraInfo, RequestContext);
            }

            return true;
        }
        
        protected void SetupBrowserView(CefWindowInfo windowInfo, int width, int height)
        {
            windowInfo.StyleEx |= WindowStyleEx.WS_EX_NOACTIVATE; // disable window activation (prevent stealing focus)
            windowInfo.SetAsChild(new CefRectangle(0, 0, width, height));
        }

        private void HandleJavascriptExecutionEngineContextCreated(CefFrame frame)
        {
        }

        private void HandleJavascriptExecutionEngineContextReleased(CefFrame frame)
        {
        }

        private void OnJavascriptExecutionEngineUncaughtException(JavascriptUncaughtExceptionEventArgs args)
        {
        }

        protected void WithErrorHandling(string scopeName, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                HandleException(scopeName, ex);
            }
        }

        protected void HandleException(string scopeName, Exception exception)
        {
            
        }

        private void OnBrowserProcessUnhandledException(MessageReceivedEventArgs e)
        {
            var exceptionDetails = Messages.UnhandledException.FromCefMessage(e.Message);
            FireBrowserProcessUnhandledExceptionHandler(exceptionDetails.ExceptionType, exceptionDetails.Message, exceptionDetails.StackTrace);
        }

        private void OnChildProcessCrashed(string message)
        {
            WithErrorHandling(nameof(OnChildProcessCrashed), () =>
            {
                var exception = SerializableException.DeserializeFromString(message);
                FireBrowserProcessUnhandledExceptionHandler(exception.ExceptionType, exception.Message, exception.StackTrace);
            });
        }

        private void FireBrowserProcessUnhandledExceptionHandler(string exceptionType, string message, string stackTrace)
        {
        }

        private void OnBrowserCreated(CefBrowser browser)
        {
           
        }

        protected virtual bool OnBrowserClose(CefBrowser browser)
        {
            if (browser.IsPopup)
            {
                // popup such as devtools, let it close its window
                return false;
            }

            Cleanup(browser);

            if (CefRuntime.Platform == CefRuntimePlatform.Linux)
            {
                // On Linux, we should return false to let cef close the browser window.
                // We shouldn't close the window directly, or disposing browser will not work as expected.
                return false;
            }
            return true;
        }

        protected void Cleanup(CefBrowser browser)
        {
            browser.Dispose();

            BrowserHost = null;
            _browser = null;
        }

        #region ICefBrowserHost

        void ICefBrowserHost.HandleBrowserCreated(CefBrowser browser)
        {
            WithErrorHandling((nameof(ICefBrowserHost.HandleBrowserDestroyed)), () =>
            {
                OnBrowserCreated(browser);
            });
        }

        void ICefBrowserHost.HandleBrowserDestroyed(CefBrowser browser)
        {
          
        }

        bool ICefBrowserHost.HandleBrowserClose(CefBrowser browser)
        {
            return false;
        }

        bool ICefBrowserHost.HandleTooltip(CefBrowser browser, string text)
        {
          

            return true;
        }

        void ICefBrowserHost.HandleAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
          
        }

        void ICefBrowserHost.HandleTitleChange(CefBrowser browser, string title)
        {
            
        }

        void ICefBrowserHost.HandleStatusMessage(CefBrowser browser, string value)
        {
        }

        bool ICefBrowserHost.HandleConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
         
            return false;
        }

        void ICefBrowserHost.HandleLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
        {
        }

        void ICefBrowserHost.HandleLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
        }

        void ICefBrowserHost.HandleLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
        }

        void ICefBrowserHost.HandleLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
        }

        void ICefBrowserHost.HandleOpenContextMenu(CefContextMenuParams parameters, CefMenuModel model, CefRunContextMenuCallback callback)
        {
        }

        void ICefBrowserHost.HandleCloseContextMenu()
        {
        }

        void ICefBrowserHost.HandleException(Exception exception)
        {
            HandleException("Unknown", exception);
        }

        bool ICefBrowserHost.HandleCursorChange(IntPtr cursorHandle, CefCursorType cursorType)
        {

            return false;        }

        void ICefBrowserHost.HandleFrameDetached(CefBrowser browser, CefFrame frame)
        {
            HandleJavascriptExecutionEngineContextReleased(frame);
        }

        #endregion
    }
}
