using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        private readonly ILogger _logger;
        private readonly IControl _control;
        private readonly IPopup _popup;

        private bool _browserCreated;

        private string _initialUrl = DefaultUrl;
        private string _title;
        private string _tooltip;
        private bool _isVisible = true;

        private CefBrowser _browser;
        private CefBrowserHost _browserHost;
        private CommonCefClient _cefClient;
        private JavascriptExecutionEngine _javascriptExecutionEngine;
        private NativeObjectMethodDispatcher _objectMethodDispatcher;
        private BaseBrowserSurface _browserSurface;

        private BuiltInRenderHandler _controlRenderHandler;
        private BuiltInRenderHandler _popupRenderHandler;

        private readonly NativeObjectRegistry _objectRegistry = new NativeObjectRegistry();

        public CommonBrowserAdapter(object eventsEmitter, string name, IControl control, IPopup popup, ILogger logger)
        {
            _eventsEmitter = eventsEmitter;
            _name = name;
            _control = control;
            _popup = popup;
            _logger = logger;

            control.ScreenInfoChanged += HandleScreenInfoChanged;
            control.VisibilityChanged += HandleVisibilityChanged;
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
            var browserHost = _browserHost;
            if (browserHost != null)
            {
                if (disposing)
                {
                    browserHost.CloseBrowser();
                }
                browserHost.Dispose();
                _browserHost = null;
            }

            var browser = _browser;
            if (browser != null)
            {
                browser.Dispose();
                _browser = null;
            }

            _browserSurface = null;

            if (disposing)
            {
                _controlRenderHandler?.Dispose();
                _popupRenderHandler?.Dispose();
                GC.SuppressFinalize(this);
            }
        }

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

        public bool AllowsTransparency { get; set; } = false;

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

        public bool IsInitialized => _browser != null;

        public bool IsLoading => _browser?.IsLoading ?? false;

        public string Title => _title;

        public double ZoomLevel
        {
            get => _browserHost?.GetZoomLevel() ?? 0;
            set => _browserHost.SetZoomLevel(value);
        }

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

                    if (_browserCreated)
                    {
                        // browser was already created, but not completely initialized, we have to queue url load
                        void OnBrowserInitialized()
                        {
                            Initialized -= OnBrowserInitialized;

                            CefRuntime.PostTask(CefThreadId.UI, new ActionTask(() =>
                            {
                                _browser.GetMainFrame()?.LoadUrl(_initialUrl);
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

            if (_browser != null)
                _browser.GetMainFrame().LoadString(content, url);
        }

        public bool CanGoBack()
        {
            if (_browser != null)
                return _browser.CanGoBack;
            else
                return false;
        }

        public void GoBack()
        {
            if (_browser != null)
                _browser.GoBack();
        }

        public bool CanGoForward()
        {
            if (_browser != null)
                return _browser.CanGoForward;
            else
                return false;
        }

        public void GoForward()
        {
            if (_browser != null)
                _browser.GoForward();
        }

        public void Reload(bool ignoreCache)
        {
            if (_browser != null)
            {
                if (ignoreCache)
                {
                    _browser.ReloadIgnoreCache();
                }
                else
                {
                    _browser.Reload();
                }
            }
        }

        public void ExecuteJavaScript(string code, string url, int line)
        {
            if (_browser != null)
                _browser.GetMainFrame().ExecuteJavaScript(code, url, line);
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url, int line, string frameName = null)
        {
            if (_browser != null)
            {
                var frame = frameName != null ? _browser.GetFrame(frameName) : _browser.GetMainFrame();
                if (frame != null)
                {
                    return EvaluateJavaScript<T>(code, url, line, frame);
                }
            }

            return Task.FromResult<T>(default(T));
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url, int line, CefFrame frame)
        {
            if (frame.IsValid)
            {
                return _javascriptExecutionEngine.Evaluate<T>(code, url, line, frame);
            }

            return Task.FromResult<T>(default(T));
        }

        private void HandleGotFocus()
        {
            WithErrorHandling(nameof(HandleGotFocus), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.SendFocusEvent(true);
                }
            });
        }

        private void HandleLostFocus()
        {
            WithErrorHandling(nameof(HandleLostFocus), () =>
            { 
                if (_browserHost != null)
                {
                    _browserHost.SendFocusEvent(false);
                }
            });
        }

        private void HandleMouseMove(CefMouseEvent mouseEvent)
        {
            WithErrorHandling(nameof(HandleMouseMove), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.SendMouseMoveEvent(mouseEvent, false);
                }
            });
        }

        private void HandleMouseLeave(CefMouseEvent mouseEvent)
        {
            WithErrorHandling(nameof(HandleMouseLeave), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.SendMouseMoveEvent(mouseEvent, true);
                }
            });
        }

        private void HandleMouseButtonDown(IControl control, CefMouseEvent mouseEvent, CefMouseButtonType mouseButton, int clickCount)
        {
            WithErrorHandling(nameof(HandleMouseButtonDown), () =>
            {
                control.Focus();
                if (_browserHost != null)
                {
                    SendMouseClickEvent(mouseEvent, mouseButton, false, clickCount);
                }
            });
        }

        private void HandleMouseButtonUp(CefMouseEvent mouseEvent, CefMouseButtonType mouseButton)
        {
            WithErrorHandling(nameof(HandleMouseButtonUp), () =>
            {
                if (_browserHost != null)
                {
                    SendMouseClickEvent(mouseEvent, mouseButton, true, 1);
                }
            });
        }

        private void HandleMouseWheel(CefMouseEvent mouseEvent, int deltaX, int deltaY)
        {
            WithErrorHandling(nameof(HandleMouseWheel), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.SendMouseWheelEvent(mouseEvent, deltaX, deltaY);
                }
            });
        }

        private void HandleTextInput(string text, out bool handled)
        {
            var _handled = false;

            WithErrorHandling(nameof(HandleMouseWheel), () =>
            {
                if (_browserHost != null)
                {
                    foreach (var c in text)
                    {
                        var keyEvent = new CefKeyEvent()
                        {
                            EventType = CefKeyEventType.Char,
                            WindowsKeyCode = c,
                            Character = c
                        };

                        _browserHost.SendKeyEvent(keyEvent);
                    }

                    _handled = true;
                }
            });

            handled = _handled;
        }

        private void HandleKeyPress(CefKeyEvent keyEvent, out bool handled)
        {
            WithErrorHandling(nameof(HandleKeyPress), () =>
            {
                if (_browserHost != null)
                {
                    //_logger.Debug(string.Format("KeyDown: system key {0}, key {1}", arg.SystemKey, arg.Key));
                    SendKeyPressEvent(keyEvent);
                }
            });
            handled = false;
        }

        private void HandleDragEnter(CefMouseEvent mouseEvent, CefDragData dragData, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDragEnter), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.DragTargetDragEnter(dragData, mouseEvent, effects);
                    _browserHost.DragTargetDragOver(mouseEvent, effects);
                }
            });
        }

        private void HandleDragOver(CefMouseEvent mouseEvent, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDragOver), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.DragTargetDragOver(mouseEvent, effects);
                }
            });

            // TODO
            //e.Effects = currentDragDropEffects;
            //e.Handled = true;
        }

        private void HandleDragLeave()
        {
            WithErrorHandling(nameof(HandleDragLeave), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.DragTargetDragLeave();
                }
            });
        }

        private void HandleDrop(CefMouseEvent mouseEvent, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDrop), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.DragTargetDragOver(mouseEvent, effects);
                    _browserHost.DragTargetDrop(mouseEvent);
                }
            });
        }

        private void HandleVisibilityChanged(bool isVisible)
        {
            if (isVisible == _isVisible)
            {
                // visiblity didn't change at all
                return;
            }
            WithErrorHandling(nameof(HandleVisibilityChanged), () =>
            {
                if (_browserHost != null)
                {
                    _isVisible = isVisible;
                    if (isVisible)
                    {
                        _browserSurface.Show();
                    }
                    else
                    {
                        _browserSurface.Hide();
                    }
                }
            });
        }

        private void HandleScreenInfoChanged(float deviceScaleFactor)
        {
            WithErrorHandling(nameof(HandleScreenInfoChanged), () =>
            {
                if (_controlRenderHandler != null) {
                    _controlRenderHandler.DeviceScaleFactor = deviceScaleFactor;
                }

                _browserHost?.NotifyScreenInfoChanged();
            });
        }

        public void CreateOrUpdateBrowser(int x, int y, int newWidth, int newHeight)
        {
            _logger.Debug("BrowserResize. H{2}xW{3}.", newHeight, newWidth);

            if (newWidth > 0 && newHeight > 0)
            {
                if (!_browserCreated)
                {
                    _browserCreated = true;

                    AttachEventHandlers(_control);
                    AttachEventHandlers(_popup);

                    // Find the window that's hosting us
                    var hParentWnd = _control.GetHostWindowHandle() ?? IntPtr.Zero;

                    var windowInfo = CefWindowInfo.Create();
                    if (CefRuntimeLoader.IsOSREnabled)
                    {
                        _controlRenderHandler = _control.CreateRenderHandler();
                        _popupRenderHandler = _popup.CreateRenderHandler();

                        _browserSurface = new BrowserOSRSurface(_controlRenderHandler);
                        _browserSurface.MoveAndResize(x, y, newWidth, newHeight);

                        windowInfo.SetAsWindowless(hParentWnd, AllowsTransparency);
                    }
                    else
                    {
                        switch (CefRuntime.Platform)
                        {
                            case CefRuntimePlatform.Windows:
                                _browserSurface = new BrowserWindowsSurface(hParentWnd);
                                break;

                            case CefRuntimePlatform.MacOSX:
                            case CefRuntimePlatform.Linux:
                                // TODO
                                throw new NotSupportedException("Standard rendering mode not supported");
                        }
                        
                        _browserSurface.MoveAndResize(x, y, newWidth, newHeight);
                        windowInfo.SetAsChild(hParentWnd, new CefRectangle(x, y, newWidth, newHeight));
                    }

                    _cefClient = new CommonCefClient(this, _logger);
                    _cefClient.Dispatcher.RegisterMessageHandler(Messages.UnhandledException.Name, OnBrowserProcessUnhandledException);

                    // This is the first time the window is being rendered, so create it.
                    CefBrowserHost.CreateBrowser(windowInfo, _cefClient, Settings, _initialUrl);
                }
                else
                {
                    var succeded = _browserSurface.MoveAndResize(x, y, newWidth, newHeight);
                    if (succeded)
                    {
                        // If the window has already been created, just resize it
                        _logger.Trace("CefBrowserHost::WasResized to {0}x{1}.", newWidth, newHeight);
                    }
                }
            }
        }

        public void ShowDeveloperTools()
        {
            var windowInfo = CefWindowInfo.Create();
            windowInfo.SetAsPopup(_browserHost.GetWindowHandle(), "DevTools");

            _browserHost.ShowDevTools(windowInfo, _browserHost.GetClient(), new CefBrowserSettings(), new CefPoint());
        }

        public void CloseDeveloperTools()
        {
            _browserHost.CloseDevTools();
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

        protected void AttachEventHandlers(IControl control)
        {
            control.GotFocus += HandleGotFocus;
            control.LostFocus += HandleLostFocus;

            control.MouseMoved += HandleMouseMove;
            control.MouseLeave += HandleMouseLeave;
            control.MouseButtonPressed += HandleMouseButtonDown;
            control.MouseButtonReleased += HandleMouseButtonUp;
            control.MouseWheelChanged += HandleMouseWheel;

            control.KeyDown += HandleKeyPress;
            control.KeyUp += HandleKeyPress;

            control.TextInput += HandleTextInput;

            control.DragEnter += HandleDragEnter;
            control.DragOver += HandleDragOver;
            control.DragLeave += HandleDragLeave;
            control.Drop += HandleDrop;
        }

        public bool IsJavascriptEngineInitialized => _javascriptExecutionEngine.IsMainFrameContextInitialized;

        public CefBrowserSettings Settings { get; } = new CefBrowserSettings();

        #region ICefBrowserHost

        void ICefBrowserHost.GetViewRect(out CefRectangle rect)
        {
            rect = GetViewRect();
        }

        protected virtual CefRectangle GetViewRect()
        {
            // The simulated screen and view rectangle are the same. This is necessary
            // for popup menus to be located and sized inside the view.
            return _browserSurface.GetViewRect();
        }

        void ICefBrowserHost.GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            GetScreenPoint(viewX, viewY, ref screenX, ref screenY);
        }

        protected void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            // TODO
            //var point = new Point(0, 0);
            //WithErrorHandling(nameof(GetScreenPoint), () =>
            //{
            //    point = _control.PointToScreen(new Point(viewX, viewY), _controlRenderHandler.DeviceScaleFactor);
            //});
            //screenX = point.X;
            //screenY = point.Y;
        }

        void ICefBrowserHost.GetScreenInfo(CefScreenInfo screenInfo)
        {
            screenInfo.DeviceScaleFactor = _controlRenderHandler.DeviceScaleFactor;
        }

        void ICefBrowserHost.HandlePopupShow(bool show)
        {
            WithErrorHandling(nameof(ICefBrowserHost.HandlePopupShow), () =>
            {
                if (show)
                {
                    _popup.Open();
                }
                else
                {
                    _popup.Close();
                }
            });
        }

        void ICefBrowserHost.HandlePopupSizeChange(CefRectangle rect)
        {
            WithErrorHandling(nameof(ICefBrowserHost.HandlePopupSizeChange), () =>
            {
                _popupRenderHandler.Resize(rect.Width, rect.Height);
                _popup.MoveAndResize(rect.X, rect.Y, rect.Width, rect.Height);
            });
        }

        void ICefBrowserHost.HandleCursorChange(IntPtr cursorHandle)
        {
            WithErrorHandling((nameof(ICefBrowserHost.HandleCursorChange)), () =>
            {
                _control.SetCursor(cursorHandle);
            });
        }

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

        protected virtual void OnBrowserCreated(CefBrowser browser)
        {
            if (_browser != null)
            {
                // Make sure we don't initialize ourselves more than once. That seems to break things.
                return;
            }

            WithErrorHandling((nameof(OnBrowserCreated)), () =>
            {
                _javascriptExecutionEngine = new JavascriptExecutionEngine(_cefClient.Dispatcher);
                _javascriptExecutionEngine.ContextCreated += OnJavascriptExecutionEngineContextCreated;
                _javascriptExecutionEngine.ContextReleased += OnJavascriptExecutionEngineContextReleased;
                _javascriptExecutionEngine.UncaughtException += OnJavascriptExecutionEngineUncaughtException;

                _objectRegistry.SetBrowser(browser);
                _objectMethodDispatcher = new NativeObjectMethodDispatcher(_cefClient.Dispatcher, _objectRegistry);

                _browser = browser;
                _browserHost = browser.GetHost();
                _browserSurface.SetBrowserHost(_browserHost);

                Initialized?.Invoke();
            });
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

        void ICefBrowserHost.HandleViewPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects, bool isPopup)
        {
            BuiltInRenderHandler renderHandler;
            if (isPopup)
            {
                renderHandler = _popupRenderHandler;
            }
            else
            {
                renderHandler = _controlRenderHandler;
            }

            const string ScopeName = nameof(ICefBrowserHost.HandleViewPaint);

            WithErrorHandling(ScopeName, () =>
            {
                renderHandler?.Paint(buffer, width, height, dirtyRects)
                              .ContinueWith(t => HandleException(ScopeName, t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            });
        }

        void ICefBrowserHost.HandleOpenContextMenu(CefContextMenuParams parameters, CefMenuModel model, CefRunContextMenuCallback callback)
        {
            _control.OpenContextMenu(MenuEntry.FromCefModel(model), parameters.X, parameters.Y, callback);
        }

        void ICefBrowserHost.HandleCloseContextMenu()
        {
            _control.CloseContextMenu();
        }

        #endregion

        private void SendMouseClickEvent(CefMouseEvent mouseEvent, CefMouseButtonType mouseButton, bool isMouseUp, int clickCount)
        {
            _browserHost.SendMouseClickEvent(mouseEvent, mouseButton, isMouseUp, clickCount);
        }

        private void SendKeyPressEvent(CefKeyEvent keyEvent)
        {
            _browserHost.SendKeyEvent(keyEvent);
        }

        void ICefBrowserHost.HandleException(Exception exception)
        {
            HandleException("Unknown", exception);
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

        private void HandleException(string scopeName, Exception exception)
        {
            _logger.ErrorException($"{_name} : Caught exception in {scopeName}()", exception);
            UnhandledException?.Invoke(_eventsEmitter, new AsyncUnhandledExceptionEventArgs(exception));
        }

        private void OnBrowserProcessUnhandledException(MessageReceivedEventArgs e)
        {
            var exceptionDetails = Messages.UnhandledException.FromCefMessage(e.Message);
            _logger.Error("Browser process unhandled exception", "Type: " + exceptionDetails.ExceptionType, "Message: " + exceptionDetails.Message, "StackTrace: " +  exceptionDetails.StackTrace);

            UnhandledException?.Invoke(
                _eventsEmitter, 
                new AsyncUnhandledExceptionEventArgs(new RenderProcessUnhandledException(exceptionDetails.ExceptionType, exceptionDetails.Message, exceptionDetails.StackTrace)));
        }
    }
}
