using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Avalonia
{
    internal class AvaloniaBrowserAdapter : CommonBrowserAdapter
    {
        protected override string Name => "AvaloniaCefBrowser";

        private readonly TemplatedControl _control;

        private WriteableBitmap _browserBitmap;

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupBitmap;

        private string _tooltip;

        private Dispatcher _mainUiDispatcher;
        private RenderHandler _renderHandler;
        private RenderHandler _popupRenderHandler;

        public AvaloniaBrowserAdapter(TemplatedControl control, ILogger logger) : base(logger)
        {
            _control = control;
            _popup = CreatePopup();

            // TODO avalonia port
            // KeyboardNavigation.SetAcceptsReturn(this, true);
            _mainUiDispatcher = Dispatcher.UIThread;
        }

        public Image BrowserImage
        {
            get
            {
                return _renderHandler?.Image;
            }
            set
            {
                _renderHandler = new RenderHandler(_mainUiDispatcher, value, _browserBitmap, _logger);
            }
        }

        protected override object EventsEmitter => _control;

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (BrowserImage != null)
                {
                    BrowserImage.Source = null;
                }

                if (_popupImage != null)
                {
                    _popupImage = null;
                }

                if (_browserBitmap != null)
                {
                    _browserBitmap.Dispose();
                    _browserBitmap = null;
                }

                if (_popupBitmap != null)
                {
                    _popupBitmap.Dispose();
                    _popupBitmap = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override IntPtr? GetHostWindowHandle()
        {
            var parentWnd = _control.GetVisualRoot() as Window;
            if (parentWnd != null)
            {
                return (IntPtr?) parentWnd.PlatformImpl.Handle.Handle;
            }

            return null;
        }

        protected override void OnPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects, bool isPopup)
        {
            RenderHandler renderHandler;
            if (isPopup)
            {
                renderHandler = _popupRenderHandler;
            }
            else
            {
                renderHandler = _renderHandler;
            }

            renderHandler?.Paint(buffer, width, height, dirtyRects);
        }

        protected override void AttachEventHandlers()
        {
            AttachEventHandlers(_control);
            AttachEventHandlers(_popup);
        }

        protected override void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            var ptScreen = new Point();

            _mainUiDispatcher.Post(
                () =>
                {
                    WithErrorHandling(nameof(GetScreenPoint), () =>
                    {
                        var ptView = new Point(viewX, viewY);
                        ptScreen = _control.PointToScreen(ptView);
                    });
                }, 
                DispatcherPriority.Normal);

            screenX = (int)ptScreen.X;
            screenY = (int)ptScreen.Y;
        }

        protected override void OnPopupShow(bool show)
        {
            if (_popup == null)
            {
                return;
            }

            _mainUiDispatcher.Post(() => _popup.IsOpen = show);
        }

        protected override void OnPopupSizeChanged(CefRectangle rect)
        {
            _mainUiDispatcher.Post(
                () =>
                {
                    _popupBitmap?.Dispose();
                    _popupBitmap = new WriteableBitmap(new PixelSize(rect.Width, rect.Height), new Vector(96, 96), PixelFormat.Bgra8888);

                    _popupImage.Source = _popupBitmap;

                    _popup.Width = rect.Width;
                    _popup.Height = rect.Height;
                    _popup.HorizontalOffset = rect.X;
                    _popup.VerticalOffset = rect.Y;

                    _popupRenderHandler = new RenderHandler(_mainUiDispatcher, _popupImage, _popupBitmap, _logger);
                });
        }

        protected override void OnCursorChanged(IntPtr cursorHandle)
        {
            var cursor = CursorsProvider.GetCursorFromHandle(cursorHandle);
            _mainUiDispatcher.Post(() => _control.Cursor = cursor);
        }
        
        protected override bool OnTooltipChanged(string text)
        {
            if (_tooltip == text)
            {
                return true;
            }

            _tooltip = text;
            UpdateTooltip(text);

            return true;
        }

        private void UpdateTooltip(string text)
        {
            // TODO BUG: sometimes the tooltips are left hanging when the user moves the cursor over the tooltip but meanwhile
            // the tooltip is destroyed
            _mainUiDispatcher.Post(
                () =>
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        ToolTip.SetIsOpen(_control, false);
                    }
                    else
                    {
                        ToolTip.SetTip(_control, text);
                        ToolTip.SetShowDelay(_control, 0);
                        ToolTip.SetIsOpen(_control, true);
                    }
                }, DispatcherPriority.Input);
        }

        protected override int RenderedWidth => _renderHandler.Width;

        protected override int RenderedHeight => _renderHandler.Height;

        private void AttachEventHandlers(InputElement target)
        {
            target.GotFocus += (sender, arg) => HandleGotFocus();
            target.LostFocus += (sender, arg) => HandleLostFocus();

            target.PointerMoved += (sender, arg) => HandleMouseMove(arg.AsCefMouseEvent());
            target.PointerLeave += (sender, arg) => HandleMouseLeave(arg.AsCefMouseEvent());
            target.PointerPressed += (sender, arg) =>
            {
                _control.Focus();
                HandleMouseButtonDown(arg.AsCefMouseEvent(), arg.MouseButton.AsCefMouseButtonType(), arg.ClickCount);
            };
            target.PointerReleased += (sender, arg) => HandleMouseButtonUp(arg.AsCefMouseEvent(), arg.MouseButton.AsCefMouseButtonType());
            target.PointerWheelChanged += (sender, arg) => HandleMouseWheel(arg.AsCefMouseEvent(), (int)arg.Delta.X, (int)arg.Delta.Y);

            target.KeyDown += (sender, arg) => HandleKeyPress(arg, false);
            target.KeyUp += (sender, arg) => HandleKeyPress(arg, true);

            target.TextInput += (sender, arg) => arg.Handled = HandleTextInput(arg.Text);
        }

        private void HandleKeyPress(KeyEventArgs args, bool isKeyUp)
        {
            HandleKeyPress(args.AsCefKeyEvent(isKeyUp));

            var key = args.Key;
            
            if (key == Key.Tab  // Avoid tabbing out the web browser control
                || key == Key.Home || key == Key.End // Prevent keyboard navigation using home and end keys
                || key == Key.Up || key == Key.Down || key == Key.Left || key == Key.Right // Prevent keyboard navigation using arrows
            )
            {
                args.Handled = true;
            }
        }

        private Popup CreatePopup()
        {
            _popupImage = CreatePopupImage();

            return new Popup
            {
                Child = _popupImage,
                PlacementTarget = _control,
                PlacementMode = PlacementMode.Bottom
            };
        }

        private Image CreatePopupImage()
        {
            return new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Source = _popupBitmap
            };
        }

        protected override void OnBrowserSizeChanged(int newWidth, int newHeight)
        {
            if (_renderHandler != null)
            {
                _renderHandler.Width = newWidth;
                _renderHandler.Height = newHeight;
            }
        }

        protected override bool IsFocused => _control.IsFocused;
    }
}
