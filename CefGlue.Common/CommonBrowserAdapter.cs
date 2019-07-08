using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.JavascriptExecution;
using Xilium.CefGlue.Common.ObjectBinding;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Common
{
    internal class CommonBrowserAdapter : ICefBrowserHost, IDisposable
    {
        private readonly string _name;
        private readonly ILogger _logger;
        private readonly IControl _control;
        private readonly IPopup _popup;

        private bool _browserCreated;

        private string _tooltip;

        // TODO
        private int _popupWidth;
        private int _popupHeight;

        private CefBrowser _browser;
        private CefBrowserHost _browserHost;
        private CommonCefClient _cefClient;
        private JavascriptExecutionEngine _javascriptExecutionEngine;
        private NativeObjectRegistry _objectRegistry;
        private NativeObjectMethodDispatcher _objectMethodDispatcher;

        public CommonBrowserAdapter(string name, IControl control, IPopup popup, ILogger logger)
        {
            _name = name;
            _control = control;
            _popup = popup;
            _logger = logger;

            StartUrl = "about:blank";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
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

            if (disposing)
            {
                RenderHandler?.Dispose();
                PopupRenderHandler?.Dispose();
            }
        }

        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;
        public event LoadingStateChangeEventHandler LoadingStateChange;
        public event LoadErrorEventHandler LoadError;

        public string StartUrl { get; set; }

        public bool AllowsTransparency { get; set; }

        public RenderHandler RenderHandler => _control.RenderHandler;

        public RenderHandler PopupRenderHandler => _popup.RenderHandler;

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

                    //_logger.Debug(string.Format("Browser_MouseMove: ({0},{1})", cursorPos.X, cursorPos.Y));
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
                    //_logger.Debug("Browser_MouseLeave");
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

                    //_logger.Debug(string.Format("Browser_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
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

                    //_logger.Debug(string.Format("Browser_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
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

            // TODO
            //if (key == Key.Tab  // Avoid tabbing out the web browser control
            //    || key == Key.Home || key == Key.End // Prevent keyboard navigation using home and end keys
            //    || key == Key.Up || key == Key.Down || key == Key.Left || key == Key.Right // Prevent keyboard navigation using arrows
            //)
            //{
            //    handled = true;
            //}
            //else
            //{
            //    handled = false;
            //}

            handled = false;
        }

        public void CreateOrUpdateBrowser(int newWidth, int newHeight)
        {
            _logger.Debug("BrowserResize. Old H{0}xW{1}; New H{2}xW{3}.", RenderedWidth, RenderedHeight, newHeight, newWidth);

            if (newWidth > 0 && newHeight > 0)
            {
                if (!_browserCreated)
                {
                    AttachEventHandlers(_control);
                    AttachEventHandlers(_popup);

                    // Create the bitmap that holds the rendered website bitmap
                    OnBrowserSizeChanged(newWidth, newHeight);

                    // Find the window that's hosting us
                    var hParentWnd = _control.GetHostWindowHandle();
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
                        OnBrowserSizeChanged(newWidth, newHeight);

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
        }

        protected int RenderedWidth => RenderHandler.Width;

        protected int RenderedHeight => RenderHandler.Height;

        protected void OnBrowserSizeChanged(int newWidth, int newHeight)
        {
            RenderHandler?.Resize(newWidth, newHeight);
        }

        #region ICefBrowserHost

        void ICefBrowserHost.GetViewRect(out CefRectangle rect)
        {
            GetViewRect(out rect);
        }

        protected virtual void GetViewRect(out CefRectangle rect)
        {
            // The simulated screen and view rectangle are the same. This is necessary
            // for popup menus to be located and sized inside the view.
            rect = new CefRectangle(0, 0, RenderedWidth, RenderedHeight);
        }

        void ICefBrowserHost.GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            GetScreenPoint(viewX, viewY, ref screenX, ref screenY);
        }

        protected void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            var point = _control.PointToScreen(new Point(viewX, viewY));
            screenX = point.X;
            screenY = point.Y;
        }

        void ICefBrowserHost.HandlePopupShow(bool show)
        {
            if (show)
            {
                _popup.Open();
            }
            else
            {
                _popup.Close();
            }
        }

        void ICefBrowserHost.HandlePopupSizeChange(CefRectangle rect)
        {
            _popup.RenderHandler.Resize(rect.Width, rect.Height);
            _popup.MoveAndResize(rect.X, rect.Y, rect.Width, rect.Height);
        }

        void ICefBrowserHost.HandleCursorChange(IntPtr cursorHandle)
        {
            _control.SetCursor(cursorHandle);
        }

        void ICefBrowserHost.HandleBrowserCreated(CefBrowser browser)
        {
            OnBrowserCreated(browser);
        }

        protected virtual void OnBrowserCreated(CefBrowser browser)
        {
            int width = 0, height = 0;

            if (_browser != null)
            {
                // Make sure we don't initialize ourselves more than once. That seems to break things.
                return;
            }
            else
            {
                _javascriptExecutionEngine = new JavascriptExecutionEngine(browser, _cefClient.Dispatcher);
                _objectRegistry = new NativeObjectRegistry(browser);
                _objectMethodDispatcher = new NativeObjectMethodDispatcher(_cefClient.Dispatcher, _objectRegistry);

                _browser = browser;
                _browserHost = browser.GetHost();
                _browserHost.SetFocus(_control.IsFocused);

                width = RenderedWidth;
                height = RenderedHeight;
            }

            if (width > 0 && height > 0)
                _browserHost.WasResized();
        }

        bool ICefBrowserHost.HandleTooltip(string text)
        {
            if (_tooltip == text)
            {
                return true;
            }

            _tooltip = text;
            _control.SetTooltip(text);

            return true;
        }

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

        void ICefBrowserHost.HandleViewPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects, bool isPopup)
        {
            RenderHandler renderHandler;
            if (isPopup)
            {
                renderHandler = PopupRenderHandler;
            }
            else
            {
                renderHandler = RenderHandler;
            }

            renderHandler?.Paint(buffer, width, height, dirtyRects);
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

        protected void WithErrorHandling(string scopeName, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.ErrorException($"{_name} : Caught exception in {scopeName}()", ex);
            }
        }
    }
}
