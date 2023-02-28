using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.InternalHandlers;
using Xilium.CefGlue.Common.Platform;
using Xilium.CefGlue.Common.Shared.Helpers;

namespace Xilium.CefGlue.Common
{
    internal class CommonOffscreenBrowserAdapter : CommonBrowserAdapter, IOffscreenCefBrowserHost
    {
        private static readonly TimeSpan ResizeDelay = TimeSpan.FromMilliseconds(50);

        private bool _isVisible = true;

        private Func<CefRectangle> _getViewRectOverride;

        public CommonOffscreenBrowserAdapter(object eventsEmitter, string name, IOffScreenControlHost control, IOffScreenPopupHost popup, ILogger logger, CefRequestContext cefRequestContext = null) 
            : base(eventsEmitter, name, control, logger, cefRequestContext) {

            Popup = popup;
        }

        protected new IOffScreenControlHost Control => (IOffScreenControlHost) base.Control;

        private IOffScreenPopupHost Popup { get; }

        protected override void InnerDispose()
        {
            Control.RenderSurface.Dispose();
            Popup.RenderSurface.Dispose();
        }

        private int Width => Control.RenderSurface.Width;
        private int Height => Control.RenderSurface.Height;

        private void SendMouseClickEvent(CefMouseEvent mouseEvent, CefMouseButtonType mouseButton, bool isMouseUp, int clickCount)
        {
            BrowserHost?.SendMouseClickEvent(mouseEvent, mouseButton, isMouseUp, clickCount);
        }

        private void HandleLostFocus()
        {
            WithErrorHandling(nameof(HandleLostFocus), () =>
            { 
                BrowserHost?.SetFocus(false);
            });
        }

        private void HandleMouseMove(CefMouseEvent mouseEvent)
        {
            WithErrorHandling(nameof(HandleMouseMove), () =>
            {
                BrowserHost?.SendMouseMoveEvent(mouseEvent, false);
            });
        }

        private void HandleMouseLeave(CefMouseEvent mouseEvent)
        {
            WithErrorHandling(nameof(HandleMouseLeave), () =>
            {
                BrowserHost?.SendMouseMoveEvent(mouseEvent, true);
            });
        }

        private void HandleMouseButtonDown(IOffScreenControlHost control, CefMouseEvent mouseEvent, CefMouseButtonType mouseButton, int clickCount)
        {
            WithErrorHandling(nameof(HandleMouseButtonDown), () =>
            {
                control.Focus();
                if (BrowserHost != null)
                {
                    SendMouseClickEvent(mouseEvent, mouseButton, false, clickCount);
                }
            });
        }

        private void HandleMouseButtonUp(CefMouseEvent mouseEvent, CefMouseButtonType mouseButton)
        {
            WithErrorHandling(nameof(HandleMouseButtonUp), () =>
            {
                if (BrowserHost != null)
                {
                    SendMouseClickEvent(mouseEvent, mouseButton, true, 1);
                }
            });
        }

        private void HandleMouseWheel(CefMouseEvent mouseEvent, int deltaX, int deltaY)
        {
            WithErrorHandling(nameof(HandleMouseWheel), () =>
            {
                BrowserHost?.SendMouseWheelEvent(mouseEvent, deltaX, deltaY);
            });
        }

        private void HandleTextInput(string text, out bool handled)
        {
            var _handled = false;

            WithErrorHandling(nameof(HandleMouseWheel), () =>
            {
                if (BrowserHost != null)
                {
                    foreach (var c in text)
                    {
                        var keyEvent = new CefKeyEvent()
                        {
                            EventType = CefKeyEventType.Char,
                            WindowsKeyCode = c,
                            Character = c
                        };

                        BrowserHost?.SendKeyEvent(keyEvent);
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
                if (BrowserHost != null)
                {
                    //_logger.Debug(string.Format("KeyDown: system key {0}, key {1}", arg.SystemKey, arg.Key));
                    BrowserHost?.SendKeyEvent(keyEvent);
                }
            });
            handled = false;
        }

        private void HandleDragEnter(CefMouseEvent mouseEvent, CefDragData dragData, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDragEnter), () =>
            {
                BrowserHost?.DragTargetDragEnter(dragData, mouseEvent, effects);
                BrowserHost?.DragTargetDragOver(mouseEvent, effects);
            });
        }

        private void HandleDragOver(CefMouseEvent mouseEvent, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDragOver), () =>
            {
                BrowserHost?.DragTargetDragOver(mouseEvent, effects);
            });

            // TODO
            //e.Effects = currentDragDropEffects;
            //e.Handled = true;
        }

        private void HandleDragLeave()
        {
            WithErrorHandling(nameof(HandleDragLeave), () =>
            {
                BrowserHost?.DragTargetDragLeave();
            });
        }

        private void HandleDrop(CefMouseEvent mouseEvent, CefDragOperationsMask effects)
        {
            WithErrorHandling(nameof(HandleDrop), () =>
            {
                BrowserHost?.DragTargetDragOver(mouseEvent, effects);
                BrowserHost?.DragTargetDrop(mouseEvent);
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
                if (BrowserHost != null)
                {
                    _isVisible = isVisible;
                    if (isVisible)
                    {
                        BrowserHost.WasHidden(false);

                        // workaround cef OSR bug (https://bitbucket.org/chromiumembedded/cef/issues/2483/osr-invalidate-does-not-generate-frame)
                        // we notify browser of a resize and return height+1px on next GetViewRect call
                        // then restore the original size back again
                        ActionTask.Run(async () =>
                        {
                            _getViewRectOverride = () =>
                            {
                                return new CefRectangle(0, 0, Width, Height + 1);
                            };
                            BrowserHost.WasResized();

                            await Task.Delay(ResizeDelay);

                            if (BrowserHost != null)
                            {
                                _getViewRectOverride = null;
                                BrowserHost.WasResized();
                            }
                        });   
                    }
                    else
                    {
                        BrowserHost.WasHidden(true);
                    }
                }
            });
        }

        private void HandleScreenInfoChanged(float deviceScaleFactor)
        {
            WithErrorHandling(nameof(HandleScreenInfoChanged), () =>
            {
                Control.RenderSurface.DeviceScaleFactor = deviceScaleFactor;
                Popup.RenderSurface.DeviceScaleFactor = deviceScaleFactor;

                BrowserHost?.WasResized();
                // Might cause a crash due to a SurfaceSync check in chromium code.
                //
                // Fixed in chromium versions >= 79.0.3909.0 (https://chromium-review.googlesource.com/c/chromium/src/+/1792459)
                //
                //_browserHost?.NotifyScreenInfoChanged();
            });
        }

        protected override void HandleControlSizeChanged(CefSize size)
        {
            if (IsBrowserCreated)
            {
                ResizeBrowser(size.Width, size.Height);
            }
            else
            {
                CreateBrowser(size.Width, size.Height);
            }
        }

        private void AttachEventHandlers(IOffScreenControlHost control)
        {
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

        protected override void SetupBrowserView(CefWindowInfo windowInfo, int width, int height, IntPtr hostViewHandle)
        {
            AttachEventHandlers(Control);
            AttachEventHandlers(Popup);

            Control.ScreenInfoChanged += HandleScreenInfoChanged;
            Control.VisibilityChanged += HandleVisibilityChanged;

            Control.RenderSurface.Resize(width, height);

            // Find the window that's hosting us
            windowInfo.SetAsWindowless(hostViewHandle, Control.RenderSurface.AllowsTransparency);
        }

        protected override void OnBrowserHostCreated(CefBrowserHost browserHost)
        {
            if (Width > 0 && Height > 0)
            {
                browserHost.WasResized();
            }
        }

        protected void ResizeBrowser(int newWidth, int newHeight)
        {
            if (Width == newWidth && Height == newHeight)
            {
                return;
            }

            Control.RenderSurface.Resize(newWidth, newHeight);
            BrowserHost?.WasResized();
            
            _logger.Debug($"Browser resized {newWidth}x{newHeight}");
        }


        protected override bool OnBrowserClose(CefBrowser browser)
        {
            Cleanup(browser);
            // According to cef documentation:
            // If no OS window exists (window rendering disabled) returning false will cause the browser object to be destroyed immediately
            return false;
        }

        #region IOffscreenCefBrowserHost

        void IOffscreenCefBrowserHost.GetViewRect(out CefRectangle rect)
        {
            rect = GetViewRect();
        }

        private CefRectangle GetViewRect()
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

        private void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
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
            screenInfo.DeviceScaleFactor = Control.RenderSurface.DeviceScaleFactor;
        }

        void IOffscreenCefBrowserHost.HandlePopupShow(bool show)
        {
            WithErrorHandling(nameof(IOffscreenCefBrowserHost.HandlePopupShow), () =>
            {
                if (show)
                {
                    Popup.Open();
                }
                else
                {
                    Popup.Close();
                }
            });
        }

        void IOffscreenCefBrowserHost.HandlePopupSizeChange(CefRectangle rect)
        {
            WithErrorHandling(nameof(IOffscreenCefBrowserHost.HandlePopupSizeChange), () =>
            {
                Popup.RenderSurface.Resize(rect.Width, rect.Height);
                Popup.MoveAndResize(rect.X, rect.Y, rect.Width, rect.Height);
            });
        }

        void IOffscreenCefBrowserHost.HandleViewPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects, bool isPopup)
        {
            if (_getViewRectOverride != null)
            {
                return;
            }

            OffScreenRenderSurface renderHandler;
            if (isPopup)
            {
                renderHandler = Popup.RenderSurface;
            }
            else
            {
                renderHandler = Control.RenderSurface;
            }

            const string ScopeName = nameof(IOffscreenCefBrowserHost.HandleViewPaint);

            WithErrorHandling(ScopeName, () =>
            {
                renderHandler?.Render(buffer, width, height, dirtyRects)
                              .ContinueWith(t => HandleException(ScopeName, t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            });
        }

        void IOffscreenCefBrowserHost.HandleStartDragging(CefBrowser browser, CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            WithErrorHandling(nameof(IOffscreenCefBrowserHost.HandleStartDragging), async () =>
            {
                var result = await Control.StartDrag(dragData, allowedOps, x, y);
                BrowserHost.DragSourceEndedAt(x, y, result);
                BrowserHost.DragSourceSystemDragEnded();
            });
        }

        void IOffscreenCefBrowserHost.HandleUpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation)
        {
            Control.UpdateDragCursor(operation);
        }

        #endregion
    }
}
