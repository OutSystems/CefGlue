using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.JavascriptExecution;
using Xilium.CefGlue.Common.ObjectBinding;
using Xilium.CefGlue.Common.Platform;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Platform.Windows;

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
        private int _maxNativeMethodsParallelCalls = int.MaxValue;
        private CefBrowser _browser;
        private CommonCefClient _cefClient;
        private PipeServer _crashServerPipe;
        private string _crashServerPipeName;
        private JavascriptExecutionEngine _javascriptExecutionEngine;
        private NativeObjectMethodDispatcher _objectMethodDispatcher;

        private readonly NativeObjectRegistry _objectRegistry = new NativeObjectRegistry();

        private object _disposeLock = new object();

        public CommonBrowserAdapter(object eventsEmitter, string name, IControl control, ILogger logger)
        {
            _eventsEmitter = eventsEmitter;
            _name = name;
            _logger = logger;

            Control = control;

            control.GotFocus += HandleGotFocus;
            control.SizeChanged += HandleControlSizeChanged;

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

            if (_logger.IsInfoEnabled)
            {
                _logger.Info($"Browser adapter disposed (Id:{GetHashCode()}");
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

        protected virtual IControl Control { get; }

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

        public bool IsJavascriptEngineInitialized => _javascriptExecutionEngine?.IsMainFrameContextInitialized == true;

        public int MaxNativeMethodsParallelCalls
        {
            get => _maxNativeMethodsParallelCalls;
            set
            {
                if (_objectMethodDispatcher != null)
                {
                    throw new InvalidOperationException($"Cannot set {nameof(MaxNativeMethodsParallelCalls)} after browser has been initialized");
                }
                _maxNativeMethodsParallelCalls = value;
            }
        }

        public CefBrowserSettings Settings { get; } = new CefBrowserSettings();

        public CefBrowser Browser => _browser;

        private void NavigateTo(string url)
        {
            // Remove leading whitespace from the URL
            url = url.TrimStart();

            /// to play safe, load url must be called after <see cref="OnBrowserCreated(CefBrowser)"/> which runs on CefThreadId.UI, 
            /// otherwise the navigation will be aborted
            ActionTask.Run(() =>
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

                            ActionTask.Run(() =>
                            {
                                _browser?.GetMainFrame()?.LoadUrl(_initialUrl);
                                _initialUrl = null;
                            });
                        }

                        Initialized += OnBrowserInitialized;
                    }
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

        public void ExecuteJavaScript(string code, string url, int line)
        {
            _browser?.GetMainFrame().ExecuteJavaScript(code, url, line);
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url, int line, string frameName = null, TimeSpan? timeout = null)
        {
            var frame = frameName != null ? _browser?.GetFrame(frameName) : _browser?.GetMainFrame();
            if (frame != null)
            {
                return EvaluateJavaScript<T>(code, url, line, frame, timeout);
            }

            return Task.FromResult<T>(default);
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url, int line, CefFrame frame, TimeSpan? timeout = null)
        {
            if (frame.IsValid && _javascriptExecutionEngine != null)
            {
                return _javascriptExecutionEngine.Evaluate<T>(code, url, line, frame, timeout);
            }

            return Task.FromResult<T>(default);
        }

        public void ShowDeveloperTools()
        {
            var windowInfo = CefWindowInfo.Create();
            if (CefRuntime.Platform != CefRuntimePlatform.MacOSX)
            {
                // don't know why but I can't do this on macosx
                windowInfo.SetAsPopup(BrowserHost?.GetWindowHandle() ?? IntPtr.Zero, "DevTools");
            }

            BrowserHost?.ShowDevTools(windowInfo, _cefClient, new CefBrowserSettings(), new CefPoint());
        }

        public void CloseDeveloperTools()
        {
            BrowserHost?.CloseDevTools();
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

        public bool CreateBrowser(int width, int height)
        {
            if (IsBrowserCreated || width < 0 || height < 0)
            {
                return false;
            }

            var hostViewHandle = Control.GetHostViewHandle();
            if (hostViewHandle == null)
            {
                return false;
            }

            IsBrowserCreated = true;

            var windowInfo = CefWindowInfo.Create();
            SetupBrowserView(windowInfo, width, height, hostViewHandle.Value);

            var cefClient = CreateCefClient();
            cefClient.Dispatcher.RegisterMessageHandler(Messages.UnhandledException.Name, OnBrowserProcessUnhandledException);
            _cefClient = cefClient;

            using (var extraInfo = CefDictionaryValue.Create())
            {
                // send the name of the crash (side) pipe to the render process
                _crashServerPipeName = Guid.NewGuid().ToString();
                extraInfo.SetString(Constants.CrashPipeNameKey, _crashServerPipeName);

                // This is the first time the window is being rendered, so create it.
                CefBrowserHost.CreateBrowser(windowInfo, cefClient, Settings, _initialUrl, extraInfo);
            }

            return true;
        }

        protected virtual CommonCefClient CreateCefClient()
        {
            return new CommonCefClient(this, null, _logger);
        }

        protected virtual void SetupBrowserView(CefWindowInfo windowInfo, int width, int height, IntPtr hostViewHandle)
        {
            windowInfo.StyleEx |= WindowStyleEx.WS_EX_NOACTIVATE; // disable window activation (prevent stealing focus)
            windowInfo.SetAsChild(hostViewHandle, new CefRectangle(0, 0, width, height));
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

        protected virtual void HandleGotFocus()
        {
            WithErrorHandling(nameof(HandleGotFocus), () =>
            {
                BrowserHost?.SetFocus(true);
            });
        }

        protected virtual void HandleControlSizeChanged(CefSize size)
        {
            var created = CreateBrowser(size.Width, size.Height);
            if (created)
            {
                Control.SizeChanged -= HandleControlSizeChanged;
            }
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
            var exception = new RenderProcessUnhandledException(exceptionType, message, stackTrace);

            _logger.ErrorException("Browser process unhandled exception", exception);

            UnhandledException?.Invoke(_eventsEmitter, new AsyncUnhandledExceptionEventArgs(exception));
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
                _crashServerPipe = new PipeServer(_crashServerPipeName);
                _crashServerPipe.MessageReceived += OnChildProcessCrashed;

                var browserHost = browser.GetHost();
                BrowserHost = browserHost;

                var dispatcher = _cefClient?.Dispatcher;
                if (dispatcher != null)
                {
                    var javascriptExecutionEngine = new JavascriptExecutionEngine(dispatcher);
                    javascriptExecutionEngine.ContextCreated += OnJavascriptExecutionEngineContextCreated;
                    javascriptExecutionEngine.ContextReleased += OnJavascriptExecutionEngineContextReleased;
                    javascriptExecutionEngine.UncaughtException += OnJavascriptExecutionEngineUncaughtException;
                    _javascriptExecutionEngine = javascriptExecutionEngine;

                    _objectRegistry.SetBrowser(browser);
                    _objectMethodDispatcher = new NativeObjectMethodDispatcher(dispatcher, _objectRegistry, MaxNativeMethodsParallelCalls);
                }

                OnBrowserHostCreated(browserHost);

                Initialized?.Invoke();
            });
        }

        protected virtual void OnBrowserHostCreated(CefBrowserHost browserHost)
        {
            Control.InitializeRender(browserHost.GetWindowHandle());
        }

        protected virtual bool OnBrowserClose(CefBrowser browser)
        {
            if (browser.IsPopup)
            {
                // popup such as devtools, let it close its window
                return false; 
            }

            Control.DestroyRender();
            Cleanup(browser);

            return true;
        }

        protected void Cleanup(CefBrowser browser)
        {
            _crashServerPipe?.Dispose();

            browser.Dispose();

            BrowserHost = null;
            _cefClient = null;
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
            WithErrorHandling((nameof(ICefBrowserHost.HandleBrowserDestroyed)), () =>
            {
                _objectMethodDispatcher?.Dispose();
                _objectMethodDispatcher = null;
            });
        }

        bool ICefBrowserHost.HandleBrowserClose(CefBrowser browser)
        {
            var result = false;
            WithErrorHandling((nameof(ICefBrowserHost.HandleBrowserClose)), () =>
            {
                result = OnBrowserClose(browser);
            });

            return result;
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
                Control.SetTooltip(text);
            });

            return true;
        }

        void ICefBrowserHost.HandleAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
            if (browser.IsPopup || !frame.IsMain)
            {
                return;
            }

            AddressChanged?.Invoke(_eventsEmitter, url);
        }

        void ICefBrowserHost.HandleTitleChange(CefBrowser browser, string title)
        {
            if (browser.IsPopup)
            {
                return;
            }

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
            Control.OpenContextMenu(MenuEntry.FromCefModel(model), parameters.X, parameters.Y, callback);
        }

        void ICefBrowserHost.HandleCloseContextMenu()
        {
            Control.CloseContextMenu();
        }
       
        void ICefBrowserHost.HandleException(Exception exception)
        {
            HandleException("Unknown", exception);
        }

        #endregion
    }
}
