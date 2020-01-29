using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.InternalHandlers;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Common
{
    internal class CommonOffscreenBrowserAdapter : CommonBrowserAdapter, IOffscreenCefBrowserHost
    {
        private bool _isVisible = true;

        private BuiltInRenderHandler _controlRenderHandler;
        private BuiltInRenderHandler _popupRenderHandler;

        private Func<CefRectangle> _getViewRectOverride;

        public CommonOffscreenBrowserAdapter(object eventsEmitter, string name, ILogger logger) 
            : base(eventsEmitter, name, logger) { }

        protected override void InnerDispose()
        {
            _controlRenderHandler?.Dispose();
            _popupRenderHandler?.Dispose();
        }

        private int Width => _controlRenderHandler?.Width ?? 0;
        private int Height => _controlRenderHandler?.Height ?? 0;


        private void SendMouseClickEvent(CefMouseEvent mouseEvent, CefMouseButtonType mouseButton, bool isMouseUp, int clickCount)
        {
            _browserHost?.SendMouseClickEvent(mouseEvent, mouseButton, isMouseUp, clickCount);
        }

        private void HandleGotFocus()
        {
            WithErrorHandling(nameof(HandleGotFocus), () =>
            {
                _browserHost?.SendFocusEvent(true);
            });
        }

        private void HandleLostFocus()
        {
            WithErrorHandling(nameof(HandleLostFocus), () =>
            { 
                _browserHost?.SendFocusEvent(false);
            });
        }

        private void HandleMouseMove(CefMouseEvent mouseEvent)
        {
            WithErrorHandling(nameof(HandleMouseMove), () =>
            {
                _browserHost?.SendMouseMoveEvent(mouseEvent, false);
            });
        }

        private void HandleMouseLeave(CefMouseEvent mouseEvent)
        {
            WithErrorHandling(nameof(HandleMouseLeave), () =>
            {
                _browserHost?.SendMouseMoveEvent(mouseEvent, true);
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
                _browserHost?.SendMouseWheelEvent(mouseEvent, deltaX, deltaY);
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

                        _browserHost?.SendKeyEvent(keyEvent);
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
                    _browserHost?.SendKeyEvent(keyEvent);
                }
            });
            handled = false;
        }

        private void HandleDragEnter(CefMouseEvent mouseEvent, CefDragData dragData, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDragEnter), () =>
            {
                _browserHost?.DragTargetDragEnter(dragData, mouseEvent, effects);
                _browserHost?.DragTargetDragOver(mouseEvent, effects);
            });
        }

        private void HandleDragOver(CefMouseEvent mouseEvent, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDragOver), () =>
            {
                _browserHost?.DragTargetDragOver(mouseEvent, effects);
            });

            // TODO
            //e.Effects = currentDragDropEffects;
            //e.Handled = true;
        }

        private void HandleDragLeave()
        {
            WithErrorHandling(nameof(HandleDragLeave), () =>
            {
                _browserHost?.DragTargetDragLeave();
            });
        }

        private void HandleDrop(CefMouseEvent mouseEvent, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDrop), () =>
            {
                _browserHost?.DragTargetDragOver(mouseEvent, effects);
                _browserHost?.DragTargetDrop(mouseEvent);
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
                        _browserHost.WasHidden(false);

                        // workaround cef OSR bug (https://bitbucket.org/chromiumembedded/cef/issues/2483/osr-invalidate-does-not-generate-frame)
                        // we notify browser of a resize and return height+1px on next GetViewRect call
                        // then restore the original size back again
                        CefRuntime.PostTask(CefThreadId.UI, new ActionTask(() =>
                        {
                            _getViewRectOverride = () =>
                            {
                                _getViewRectOverride = null;
                                _browserHost?.WasResized();
                                return new CefRectangle(0, 0, Width, Height + 1);
                            };

                            _browserHost.WasResized();
                        }));
                    }
                    else
                    {
                        _browserHost.WasHidden(true);
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

                // Might cause a crash due to a SurfaceSync check in chromium code.
                //
                // Fixed in chromium versions >= 79.0.3909.0 (https://chromium-review.googlesource.com/c/chromium/src/+/1792459)
                //
                //_browserHost?.NotifyScreenInfoChanged();
            });
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

        protected override CommonCefClient CreateCefClient()
        {
            return new CommonCefClient(this, new CommonCefRenderHandler(this, _logger), _logger);
        }

        protected override void CreateBrowser(CefWindowInfo windowInfo, int x, int y, int newWidth, int newHeight)
        {
            AttachEventHandlers(_control);
            AttachEventHandlers(_popup);

            _control.ScreenInfoChanged += HandleScreenInfoChanged;
            _control.VisibilityChanged += HandleVisibilityChanged;

            _controlRenderHandler = _control.CreateRenderHandler();
            _popupRenderHandler = _popup.CreateRenderHandler();

            // TODO _browserSurface?.MoveAndResize(x, y, newWidth, newHeight);

            // Find the window that's hosting us
            var windowHandle = _control.GetHostWindowHandle() ?? IntPtr.Zero;
            windowInfo.SetAsWindowless(windowHandle, _controlRenderHandler.AllowsTransparency);
        }

        protected override void OnBrowserHostCreated(CefBrowserHost browserHost)
        {
            if (Width > 0 && Height > 0)
            {
                browserHost.WasResized();
            }
        }

        protected override bool ResizeBrowser(int x, int y, int newWidth, int newHeight)
        {
            if (Width == newWidth && Height == newHeight)
            {
                return false;
            }

            _controlRenderHandler.Resize(newWidth, newHeight);
            _browserHost?.WasResized();

            return true;
        }

        #region ICefBrowserHost

        void IOffscreenCefBrowserHost.GetViewRect(out CefRectangle rect)
        {
            rect = GetViewRect();
        }

        protected CefRectangle GetViewRect()
        {
            var result = _getViewRectOverride?.Invoke() ?? new CefRectangle(0, 0, Width, Height);
            // The simulated screen and view rectangle are the same. This is necessary
            // for popup menus to be located and sized inside the view.
            if (result.Width <= 0 || result.Height <= 0)
            {
                // NOTE: width and height must be > 0, otherwise cef will blow up
                return new CefRectangle(0, 0, Math.Max(1, result.Width), Math.Max(1, result.Height));
            }
            return result;
        }

        void IOffscreenCefBrowserHost.GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
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

        void IOffscreenCefBrowserHost.GetScreenInfo(CefScreenInfo screenInfo)
        {
            screenInfo.DeviceScaleFactor = _controlRenderHandler.DeviceScaleFactor;
        }

        void IOffscreenCefBrowserHost.HandlePopupShow(bool show)
        {
            WithErrorHandling(nameof(IOffscreenCefBrowserHost.HandlePopupShow), () =>
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

        void IOffscreenCefBrowserHost.HandlePopupSizeChange(CefRectangle rect)
        {
            WithErrorHandling(nameof(IOffscreenCefBrowserHost.HandlePopupSizeChange), () =>
            {
                _popupRenderHandler.Resize(rect.Width, rect.Height);
                _popup.MoveAndResize(rect.X, rect.Y, rect.Width, rect.Height);
            });
        }

        void IOffscreenCefBrowserHost.HandleCursorChange(IntPtr cursorHandle)
        {
            WithErrorHandling((nameof(IOffscreenCefBrowserHost.HandleCursorChange)), () =>
            {
                _control.SetCursor(cursorHandle);
            });
        }

        void IOffscreenCefBrowserHost.HandleViewPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects, bool isPopup)
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

            const string ScopeName = nameof(IOffscreenCefBrowserHost.HandleViewPaint);

            WithErrorHandling(ScopeName, () =>
            {
                renderHandler?.Paint(buffer, width, height, dirtyRects)
                              .ContinueWith(t => HandleException(ScopeName, t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            });
        }

        void IOffscreenCefBrowserHost.HandleStartDragging(CefBrowser browser, CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            WithErrorHandling(nameof(IOffscreenCefBrowserHost.HandleStartDragging), async () =>
            {
                var result = await _control.StartDragging(dragData, allowedOps, x, y);
                _browserHost.DragSourceEndedAt(x, y, result);
                _browserHost.DragSourceSystemDragEnded();
            });
        }

        void IOffscreenCefBrowserHost.HandleUpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation)
        {
            _control.UpdateDragCursor(operation);
        }

        #endregion
    }
}
