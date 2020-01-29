using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.JavascriptExecution;
using Xilium.CefGlue.Common.ObjectBinding;
using Xilium.CefGlue.Common.Platform;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common
{
    internal class CommonBrowserAdapter : ICefBrowserHost, IDisposable
    {
        private const string DefaultUrl = "about:blank";

        private readonly object _eventsEmitter;
        private readonly string _name;
        protected readonly ILogger _logger;

        private string _initialUrl = DefaultUrl;
        private string _title;
        private string _tooltip;

        private CefBrowser _browser;
        protected CefBrowserHost _browserHost;

        private CommonCefClient _cefClient;
        private JavascriptExecutionEngine _javascriptExecutionEngine;
        private NativeObjectMethodDispatcher _objectMethodDispatcher;

        protected IControl _control;
        protected IPopup _popup;

        private readonly NativeObjectRegistry _objectRegistry = new NativeObjectRegistry();

        public static CommonBrowserAdapter CreateInstance(object eventsEmitter, string name, ILogger logger)
        {
            if (CefRuntimeLoader.IsOSREnabled)
            {
                return new CommonOffscreenBrowserAdapter(eventsEmitter, name, logger);
            }

            return new CommonBrowserAdapter(eventsEmitter, name, logger);
        }

        protected CommonBrowserAdapter(object eventsEmitter, string name, ILogger logger)
        {
            _eventsEmitter = eventsEmitter;
            _name = name;
            _logger = logger;

            if (_logger.IsInfoEnabled)
            {
                _logger.Info($"Browser adapter created (Id:{GetHashCode()}");
            }
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
            if (_logger.IsInfoEnabled)
            {
                _logger.Info($"Browser adapter disposed (Id:{GetHashCode()}");
            }

            var browserHost = _browserHost;
            if (browserHost != null)
            {
                _browserHost = null;
                browserHost.CloseBrowser(true);
                browserHost.Dispose();
            }

            var browser = _browser;
            if (browser != null)
            {
                _browser = null;
                browser.Dispose();
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

        public bool IsBrowserCreated { get; private set; }

        public bool IsInitialized => _browser != null;

        public bool IsLoading => _browser?.IsLoading ?? false;

        public string Title => _title;

        public double ZoomLevel
        {
            get => _browserHost?.GetZoomLevel() ?? 0;
            set => _browserHost?.SetZoomLevel(value);
        }

        public bool IsJavascriptEngineInitialized => _javascriptExecutionEngine.IsMainFrameContextInitialized;

        public CefBrowserSettings Settings { get; } = new CefBrowserSettings();

        public CefBrowser Browser => _browser;

        private void NavigateTo(string url)
        {
            // Remove leading whitespace from the URL
            url = url.TrimStart();

            /// to play safe, load url must be called after <see cref="OnBrowserCreated(CefBrowser)"/> which runs on CefThreadId.UI, 
            /// otherwise the navigation will be aborted
            CefRuntime.PostTask(CefThreadId.UI, new ActionTask(() =>
            {
                if (_browser != null)
                {
                    _browser?.GetMainFrame()?.LoadUrl(url);
                }
                else if (!string.IsNullOrEmpty(url))
                {
                    _initialUrl = url;

                    if (IsBrowserCreated)
                    {
                        // browser was already created, but not completely initialized, we have to queue url load
                        void OnBrowserInitialized()
                        {
                            Initialized -= OnBrowserInitialized;

                            CefRuntime.PostTask(CefThreadId.UI, new ActionTask(() =>
                            {
                                _browser?.GetMainFrame()?.LoadUrl(_initialUrl);
                                _initialUrl = null;
                            }));
                        }

                        Initialized += OnBrowserInitialized;
                    }
                }
            }));
        }

        public void LoadString(string content, string url)
        {
            // Remove leading whitespace from the URL
            url = url.TrimStart();

            _browser?.GetMainFrame().LoadString(content, url);
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

        public void ExecuteJavaScript(string code, string url, int line)
        {
            _browser?.GetMainFrame().ExecuteJavaScript(code, url, line);
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url, int line, string frameName = null)
        {
            var frame = frameName != null ? _browser?.GetFrame(frameName) : _browser?.GetMainFrame();
            if (frame != null)
            {
                return EvaluateJavaScript<T>(code, url, line, frame);
            }

            return Task.FromResult<T>(default(T));
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url, int line, CefFrame frame)
        {
            if (frame.IsValid && _javascriptExecutionEngine != null)
            {
                return _javascriptExecutionEngine.Evaluate<T>(code, url, line, frame);
            }

            return Task.FromResult<T>(default(T));
        }

        public void CreateBrowser(int x, int y, int newWidth, int newHeight, IControl control, IPopup popup)
        {
            _control = control;
            _popup = popup;
            
            CreateOrUpdateBrowser(x, y, newWidth, newHeight);
        }

        public void UpdateBrowser(int x, int y, int newWidth, int newHeight)
        {
            CreateOrUpdateBrowser(x, y, newWidth, newHeight);
        }

        private void CreateOrUpdateBrowser(int x, int y, int newWidth, int newHeight)
        {
            _logger.Debug($"Browser resized {newWidth}x{newHeight}");

            if (newWidth > 0 && newHeight > 0)
            {
                if (!IsBrowserCreated)
                {
                    IsBrowserCreated = true;

                    var windowInfo = CefWindowInfo.Create();
                    CreateBrowser(windowInfo, x, y, newWidth, newHeight);

                    var cefClient = CreateCefClient();
                    cefClient.Dispatcher.RegisterMessageHandler(Messages.UnhandledException.Name, OnBrowserProcessUnhandledException);
                    _cefClient = cefClient;

                    // This is the first time the window is being rendered, so create it.
                    CefBrowserHost.CreateBrowser(windowInfo, cefClient, Settings, _initialUrl);
                }
                else
                {
                    var succeded = ResizeBrowser(x, y, newWidth, newHeight);
                    if (succeded)
                    {
                        // If the window has already been created, just resize it
                        _logger.Debug($"Browser resized {newWidth}x{newHeight}");
                    }
                }
            }
        }

        protected virtual CommonCefClient CreateCefClient()
        {
            return new CommonCefClient(this, null, _logger);
        }

        protected virtual void CreateBrowser(CefWindowInfo windowInfo, int x, int y, int newWidth, int newHeight)
        {
            // Find the view that's hosting us
            var viewHandle = _control.GetHostViewHandle() ?? IntPtr.Zero;
            //switch (CefRuntime.Platform)
            //{
            //    case CefRuntimePlatform.Windows:
            //        _browserSurface = new BrowserWindowsSurface(viewHandle, _control);
            //        break;

            //    case CefRuntimePlatform.MacOSX:
            //        _browserSurface = new BrowserMacOSSurface();
            //        break;

            //    case CefRuntimePlatform.Linux:
            //        // TODO
            //        throw new NotSupportedException("Standard rendering mode not supported");
            //}

            //_browserSurface?.MoveAndResize(x, y, newWidth, newHeight);
            windowInfo.SetAsChild(viewHandle, new CefRectangle(0, 0, newWidth, newHeight));
        }

        protected virtual bool ResizeBrowser(int x, int y, int newWidth, int newHeight)
        {
            return true;
        }

        public void ShowDeveloperTools()
        {
            var windowInfo = CefWindowInfo.Create();
            if (CefRuntime.Platform != CefRuntimePlatform.MacOSX)
            {
                // don't know why but I can't do this on macosx
                windowInfo.SetAsPopup(_browserHost?.GetWindowHandle() ?? IntPtr.Zero, "DevTools");
            }

            _browserHost?.ShowDevTools(windowInfo, _cefClient, new CefBrowserSettings(), new CefPoint());
        }

        public void CloseDeveloperTools()
        {
            _browserHost?.CloseDevTools();
        }

        public void RegisterJavascriptObject(object targetObject, string name, JavascriptObjectMethodCallHandler methodHandler = null)
        {
            _objectRegistry.Register(targetObject, name, methodHandler);
        }

        public void UnregisterJavascriptObject(string name)
        {
            _objectRegistry.Unregister(name);
        }

        public bool IsJavascriptObjectRegistered(string name)
        {
            return _objectRegistry.Get(name) != null;
        }

        private void OnJavascriptExecutionEngineContextCreated(CefFrame frame)
        {
            JavascriptContextCreated?.Invoke(_eventsEmitter, new JavascriptContextLifetimeEventArgs(frame));
        }

        private void OnJavascriptExecutionEngineContextReleased(CefFrame frame)
        {
            JavascriptContextReleased?.Invoke(_eventsEmitter, new JavascriptContextLifetimeEventArgs(frame));
        }

        private void OnJavascriptExecutionEngineUncaughtException(JavascriptUncaughtExceptionEventArgs args)
        {
            JavascriptUncaughtException?.Invoke(_eventsEmitter, args);
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
            _logger.ErrorException($"{_name} : Caught exception in {scopeName}()", exception);
            UnhandledException?.Invoke(_eventsEmitter, new AsyncUnhandledExceptionEventArgs(exception));
        }

        private void OnBrowserProcessUnhandledException(MessageReceivedEventArgs e)
        {
            var exceptionDetails = Messages.UnhandledException.FromCefMessage(e.Message);
            var exception = new RenderProcessUnhandledException(exceptionDetails.ExceptionType, exceptionDetails.Message, exceptionDetails.StackTrace);

            _logger.ErrorException("Browser process unhandled exception", exception);

            UnhandledException?.Invoke(
                _eventsEmitter,
                new AsyncUnhandledExceptionEventArgs(new RenderProcessUnhandledException(exceptionDetails.ExceptionType, exceptionDetails.Message, exceptionDetails.StackTrace)));
        }

        private void OnBrowserCreated(CefBrowser browser)
        {
            if (_browser != null)
            {
                // Make sure we don't initialize ourselves more than once. That seems to break things.
                return;
            }

            WithErrorHandling((nameof(OnBrowserCreated)), () =>
            {
                _browser = browser;

                var browserHost = browser.GetHost();
                _browserHost = browserHost;

                var dispatcher = _cefClient?.Dispatcher;
                if (dispatcher != null)
                {
                    var javascriptExecutionEngine = new JavascriptExecutionEngine(dispatcher);
                    javascriptExecutionEngine.ContextCreated += OnJavascriptExecutionEngineContextCreated;
                    javascriptExecutionEngine.ContextReleased += OnJavascriptExecutionEngineContextReleased;
                    javascriptExecutionEngine.UncaughtException += OnJavascriptExecutionEngineUncaughtException;
                    _javascriptExecutionEngine = javascriptExecutionEngine;

                    _objectRegistry.SetBrowser(browser);
                    _objectMethodDispatcher = new NativeObjectMethodDispatcher(dispatcher, _objectRegistry);
                }

                OnBrowserHostCreated(browserHost);

                Initialized?.Invoke();
            });
        }

        protected virtual void OnBrowserHostCreated(CefBrowserHost browserHost) {
            _control.InitializeRender(browserHost.GetWindowHandle());
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
            WithErrorHandling((nameof(ICefBrowserHost.HandleBrowserDestroyed)), () =>
            {
                _objectMethodDispatcher?.Dispose();
                _objectMethodDispatcher = null;
            });
        }

        bool ICefBrowserHost.HandleTooltip(CefBrowser browser, string text)
        {
            WithErrorHandling((nameof(ICefBrowserHost.HandleTooltip)), () =>
            {
                if (_tooltip == text)
                {
                    return;
                }

                _tooltip = text;
                _control.SetTooltip(text);
            });

            return true;
        }

        void ICefBrowserHost.HandleAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
            AddressChanged?.Invoke(_eventsEmitter, url);
        }

        void ICefBrowserHost.HandleTitleChange(CefBrowser browser, string title)
        {
            _title = title;
            TitleChanged?.Invoke(_eventsEmitter, title);
        }

        void ICefBrowserHost.HandleStatusMessage(CefBrowser browser, string value)
        {
            StatusMessage?.Invoke(_eventsEmitter, value);
        }

        bool ICefBrowserHost.HandleConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            var handler = ConsoleMessage;
            if (handler != null)
            {
                var args = new ConsoleMessageEventArgs(level, message, source, line);
                ConsoleMessage?.Invoke(_eventsEmitter, args);
                return !args.OutputToConsole;
            }
            return false;
        }

        void ICefBrowserHost.HandleLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
        {
            LoadStart?.Invoke(_eventsEmitter, new LoadStartEventArgs(frame));
        }

        void ICefBrowserHost.HandleLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            LoadEnd?.Invoke(_eventsEmitter, new LoadEndEventArgs(frame, httpStatusCode));
        }

        void ICefBrowserHost.HandleLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
            LoadError?.Invoke(_eventsEmitter, new LoadErrorEventArgs(frame, errorCode, errorText, failedUrl));
        }

        void ICefBrowserHost.HandleLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
            LoadingStateChange?.Invoke(_eventsEmitter, new LoadingStateChangeEventArgs(isLoading, canGoBack, canGoForward));
        }

        void ICefBrowserHost.HandleOpenContextMenu(CefContextMenuParams parameters, CefMenuModel model, CefRunContextMenuCallback callback)
        {
            _control.OpenContextMenu(MenuEntry.FromCefModel(model), parameters.X, parameters.Y, callback);
        }

        void ICefBrowserHost.HandleCloseContextMenu()
        {
            _control.CloseContextMenu();
        }
       
        void ICefBrowserHost.HandleException(Exception exception)
        {
            HandleException("Unknown", exception);
        }

        #endregion
    }
}
