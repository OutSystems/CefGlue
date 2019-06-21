using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.JavascriptExecution;
using Xilium.CefGlue.Common.ObjectBinding;

namespace Xilium.CefGlue.Common
{
    internal abstract class CommonBrowserAdapter : ICefBrowserHost, IDisposable
    {
        protected readonly ILogger _logger;

        protected int _browserWidth;
        protected int _browserHeight;
        private bool _browserCreated;
        protected bool _browserSizeChanged;

        private CefBrowser _browser;
        private CefBrowserHost _browserHost;
        private CommonCefClient _cefClient;
        private JavascriptExecutionEngine _javascriptExecutionEngine;
        private NativeObjectRegistry _objectRegistry;
        private NativeObjectMethodDispatcher _objectMethodDispatcher;

        public CommonBrowserAdapter(ILogger logger)
        {
            this._logger = logger;

            StartUrl = "about:blank";
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
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

        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;
        public event LoadingStateChangeEventHandler LoadingStateChange;
        public event LoadErrorEventHandler LoadError;

        protected abstract string Name { get; }

        protected abstract object EventsEmitter { get; }

        public string StartUrl { get; set; }

        public bool AllowsTransparency { get; set; }

        public void NavigateTo(string url)
        {
            // Remove leading whitespace from the URL
            url = url.TrimStart();

            if (_browser != null)
                _browser.GetMainFrame().LoadUrl(url);
            else
                StartUrl = url;
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

        public void Refresh()
        {
            if (_browser != null)
                _browser.Reload();
        }

        public void ExecuteJavaScript(string code, string url, int line)
        {
            if (_browser != null)
                _browser.GetMainFrame().ExecuteJavaScript(code, url, line);
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url, int line)
        {
            if (_browser == null)
            {
                return Task.FromResult<T>(default(T));
            }

            return _javascriptExecutionEngine.Evaluate<T>(code, url, line);
        }

        public void HandleGotFocus()
        {
            WithErrorHandling(nameof(HandleGotFocus), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.SendFocusEvent(true);
                }
            });
        }

        public void HandleLostFocus()
        {
            WithErrorHandling(nameof(HandleLostFocus), () =>
            { 
                if (_browserHost != null)
                {
                    _browserHost.SendFocusEvent(false);
                }
            });
        }

        public void HandleMouseMove(CefMouseEvent mouseEvent)
        {
            WithErrorHandling(nameof(HandleMouseMove), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.SendMouseMoveEvent(mouseEvent, false);

                    //_logger.Debug(string.Format("Browser_MouseMove: ({0},{1})", cursorPos.X, cursorPos.Y));
                }
            });
        }

        public void HandleMouseLeave(CefMouseEvent mouseEvent)
        {
            WithErrorHandling(nameof(HandleMouseLeave), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.SendMouseMoveEvent(mouseEvent, true);
                    //_logger.Debug("Browser_MouseLeave");
                }
            });
        }

        public void HandleMouseButtonDown(CefMouseEvent mouseEvent, CefMouseButtonType mouseButton, int clickCount)
        {
            WithErrorHandling(nameof(HandleMouseButtonDown), () =>
            {
                if (_browserHost != null)
                {
                    SendMouseClickEvent(mouseEvent, mouseButton, false, clickCount);

                    //_logger.Debug(string.Format("Browser_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
                }
            });
        }

        public void HandleMouseButtonUp(CefMouseEvent mouseEvent, CefMouseButtonType mouseButton)
        {
            WithErrorHandling(nameof(HandleMouseButtonUp), () =>
            {
                if (_browserHost != null)
                {
                    SendMouseClickEvent(mouseEvent, mouseButton, true, 1);

                    //_logger.Debug(string.Format("Browser_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
                }
            });
        }

        public void HandleMouseWheel(CefMouseEvent mouseEvent, int deltaX, int deltaY)
        {
            WithErrorHandling(nameof(HandleMouseWheel), () =>
            {
                if (_browserHost != null)
                {
                    _browserHost.SendMouseWheelEvent(mouseEvent, deltaX, deltaY);
                }
            });
        }

        public bool HandleTextInput(string text)
        {
            var handled = false;
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

                    handled = true;
                }
            });

            return handled;
        }

        public void HandleKeyPress(CefKeyEvent keyEvent)
        {
            WithErrorHandling(nameof(HandleKeyPress), () =>
            {
                if (_browserHost != null)
                {
                    //_logger.Debug(string.Format("KeyDown: system key {0}, key {1}", arg.SystemKey, arg.Key));
                    SendKeyPressEvent(keyEvent);
                }
            });
        }

        public void CreateOrUpdateBrowser(int newWidth, int newHeight)
        {
            _logger.Debug("BrowserResize. Old H{0}xW{1}; New H{2}xW{3}.", _browserHeight, _browserWidth, newHeight, newWidth);

            if (newWidth > 0 && newHeight > 0)
            {
                if (!_browserCreated)
                {
                    AttachEventHandlers();

                    // Create the bitmap that holds the rendered website bitmap
                    _browserWidth = newWidth;
                    _browserHeight = newHeight;
                    _browserSizeChanged = true;

                    // Find the window that's hosting us
                    var hParentWnd = GetHostWindowHandle();
                    if (hParentWnd != null)
                    {
                        var windowInfo = CefWindowInfo.Create();
                        windowInfo.SetAsWindowless(hParentWnd.Value, AllowsTransparency);

                        var settings = new CefBrowserSettings();
                        _cefClient = new CommonCefClient(this);

                        // This is the first time the window is being rendered, so create it.
                        CefBrowserHost.CreateBrowser(windowInfo, _cefClient, settings, string.IsNullOrEmpty(StartUrl) ? "about:blank" : StartUrl);

                        _browserCreated = true;
                    }
                }
                else
                {
                    // Only update the bitmap if the size has changed
                    if (RenderedWidth != newWidth || RenderedHeight != newHeight)
                    {
                        _browserWidth = newWidth;
                        _browserHeight = newHeight;
                        _browserSizeChanged = true;

                        // If the window has already been created, just resize it
                        if (_browserHost != null)
                        {
                            _logger.Trace("CefBrowserHost::WasResized to {0}x{1}.", newWidth, newHeight);
                            _browserHost.WasResized();
                        }
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

        public void RegisterJavascriptObject(object targetObject, string name)
        {
            _objectRegistry.Register(targetObject, name);
        }

        protected abstract IntPtr? GetHostWindowHandle();

        protected abstract void AttachEventHandlers();

        protected abstract int RenderedWidth { get; }

        protected abstract int RenderedHeight { get; }

        #region ICefBrowserHost

        void ICefBrowserHost.GetViewRect(out CefRectangle rect)
        {
            GetViewRect(out rect);
        }

        protected virtual void GetViewRect(out CefRectangle rect)
        {
            // The simulated screen and view rectangle are the same. This is necessary
            // for popup menus to be located and sized inside the view.
            rect = new CefRectangle(0, 0, (int)_browserWidth, (int)_browserHeight);
        }

        void ICefBrowserHost.GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            GetScreenPoint(viewX, viewY, ref screenX, ref screenY);
        }

        protected abstract void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY);

        void ICefBrowserHost.HandlePopupShow(bool show)
        {
            OnPopupShow(show);
        }

        protected abstract void OnPopupShow(bool show);

        void ICefBrowserHost.HandlePopupSizeChange(CefRectangle rect)
        {
            OnPopupSizeChanged(rect);
        }

        protected abstract void OnPopupSizeChanged(CefRectangle rect);

        void ICefBrowserHost.HandlePopupPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            OnPopupPaint(buffer, width, height, dirtyRects);
        }

        protected abstract void OnPopupPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects);

        void ICefBrowserHost.HandleCursorChange(IntPtr cursorHandle)
        {
            OnCursorChanged(cursorHandle);
        }

        protected abstract void OnCursorChanged(IntPtr cursorHandle);

        void ICefBrowserHost.HandleBrowserCreated(CefBrowser browser)
        {
            OnBrowserCreated(browser);
        }

        protected virtual void OnBrowserCreated(CefBrowser browser)
        {
            int width = 0, height = 0;

            //bool hasAlreadyBeenInitialized = false;

            //_mainUiDispatcher.Post(new Action(delegate
            //{
            if (_browser != null)
            {
                //hasAlreadyBeenInitialized = true;
            }
            else
            {
                _javascriptExecutionEngine = new JavascriptExecutionEngine(browser, _cefClient.Dispatcher);
                _objectRegistry = new NativeObjectRegistry(browser);
                _objectMethodDispatcher = new NativeObjectMethodDispatcher(_cefClient.Dispatcher, _objectRegistry);

                _browser = browser;
                _browserHost = browser.GetHost();
                // _browserHost.SetFocus(IsFocused);

                width = (int)_browserWidth;
                height = (int)_browserHeight;
            }
            //}), DispatcherPriority.Normal);

            // Make sure we don't initialize ourselves more than once. That seems to break things.
            //if (hasAlreadyBeenInitialized)
            //    return;

            if (width > 0 && height > 0)
                _browserHost.WasResized();

            // 			mainUiDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            // 			{
            // 				if (!string.IsNullOrEmpty(this.initialUrl))
            // 				{
            // 					NavigateTo(this.initialUrl);
            // 					this.initialUrl = string.Empty;
            // 				}
            // 			}));
        }

        bool ICefBrowserHost.HandleTooltip(string text)
        {
            return OnTooltipChanged(text);
        }

        protected abstract bool OnTooltipChanged(string text);

        void ICefBrowserHost.HandleLoadStart(LoadStartEventArgs args)
        {
            LoadStart?.Invoke(this, args);
        }

        void ICefBrowserHost.HandleLoadEnd(LoadEndEventArgs args)
        {
            LoadEnd?.Invoke(this, args);
        }

        void ICefBrowserHost.HandleLoadError(LoadErrorEventArgs args)
        {
            LoadError?.Invoke(this, args);
        }

        void ICefBrowserHost.HandleLoadingStateChange(LoadingStateChangeEventArgs args)
        {
            LoadingStateChange?.Invoke(this, args);
        }

        void ICefBrowserHost.HandleViewPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            // When browser size changed - we just skip frame updating.
            // This is dirty precheck to do not do Invoke whenever is possible.
            if (_browserSizeChanged && (width != _browserWidth || height != _browserHeight))
                return;

            OnPaint(buffer, width, height, dirtyRects);
        }

        protected abstract void OnPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects);

        #endregion

        private void SendMouseClickEvent(CefMouseEvent mouseEvent, CefMouseButtonType mouseButton, bool isMouseUp, int clickCount)
        {
            _browserHost.SendMouseClickEvent(mouseEvent, mouseButton, isMouseUp, clickCount);
        }

        private void SendKeyPressEvent(CefKeyEvent keyEvent)
        {
            _browserHost.SendKeyEvent(keyEvent);
        }

        protected void WithErrorHandling(string scopeName, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.ErrorException($"{Name} : Caught exception in {scopeName}()", ex);
            }
        }
    }
}
