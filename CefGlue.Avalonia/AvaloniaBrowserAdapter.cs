using System;
using System.Runtime.InteropServices;
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

        private Image _browserImage;
        private WriteableBitmap _browserBitmap;

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupImageBitmap;

        private string _tooltip;

        private Dispatcher _mainUiDispatcher;

        public AvaloniaBrowserAdapter(TemplatedControl control, ILogger logger) : base(logger)
        {
            _control = control;
            _popup = CreatePopup();

            // TODO avalonia port
            // KeyboardNavigation.SetAcceptsReturn(this, true);
            _mainUiDispatcher = Dispatcher.UIThread;
        }

        public Image BrowserImage { get => _browserImage; set => _browserImage = value; }

        protected override object EventsEmitter => _control;

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_browserImage != null)
                {
                    _browserImage.Source = null;
                    _browserImage = null;
                }

                if (_browserBitmap != null)
                {
                    _browserBitmap = null;
                }

                // 					if (this.browserPageD3dImage != null)
                // 						this.browserPageD3dImage = null;
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

        protected override void OnPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            _mainUiDispatcher.InvokeAsync(new Action(delegate
            {
                // Actual browser size changed check.
                if (_browserSizeChanged && (width != _browserWidth || height != _browserHeight))
                    return;

                WithErrorHandling(nameof(OnPaint), () =>
                {
                    if (_browserSizeChanged)
                    {
                        // TODO handle trasparency
                        _browserBitmap = new WriteableBitmap(new PixelSize(_browserWidth, _browserHeight), new Vector(96, 96), PixelFormat.Bgra8888);
                        _browserImage.Source = _browserBitmap;

                        _browserSizeChanged = false;
                    }

                    if (_browserBitmap != null)
                    {
                        Paint(_browserBitmap, buffer, width, height, dirtyRects);

                        _browserImage.InvalidateVisual();
                    }

                });
            }));
        }

        protected override void AttachEventHandlers()
        {
            AttachEventHandlers(_control);
            AttachEventHandlers(_popup);
        }

        protected override void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            var ptScreen = new Point();

            _mainUiDispatcher.Post(new Action(delegate
            {
                WithErrorHandling(nameof(GetScreenPoint), () =>
                {
                    var ptView = new Point(viewX, viewY);
                    ptScreen = _control.PointToScreen(ptView);
                });
            }), DispatcherPriority.Normal);

            screenX = (int)ptScreen.X;
            screenY = (int)ptScreen.Y;
        }

        protected override void OnPopupShow(bool show)
        {
            if (_popup == null)
            {
                return;
            }

            _mainUiDispatcher.Post(new Action(() => _popup.IsOpen = show));
        }

        protected override void OnPopupSizeChanged(CefRectangle rect)
        {
            _mainUiDispatcher.Post(
                new Action(
                    () =>
                    {
                        _popupImageBitmap = null;
                        _popupImageBitmap = new WriteableBitmap(new PixelSize(rect.Width, rect.Height), new Vector(96, 96), PixelFormat.Bgra8888);

                        _popupImage.Source = this._popupImageBitmap;

                        _popup.Width = rect.Width;
                        _popup.Height = rect.Height;
                        _popup.HorizontalOffset = rect.X;
                        _popup.VerticalOffset = rect.Y;
                    }));
        }

        protected override void OnPopupPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            // TODO avalonia port
            throw new NotImplementedException();
        }

        protected override void OnCursorChanged(IntPtr cursorHandle)
        {
            // TODO avalonia port
            //_mainUiDispatcher.Post(
            //    new Action(
            //        () =>
            //        {
            //            var cursor = CursorInteropHelper.Create(new SafeFileHandle(cursorHandle, false));
            //            Cursor = cursor;
            //        }));
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
                new Action(
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
                    }), DispatcherPriority.Input);
        }

        protected override int RenderedWidth => (int) _browserBitmap.Size.Width;

        protected override int RenderedHeight => (int) _browserBitmap.Size.Height;

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
            var popup = new Popup
            {
                Child = this._popupImage = CreatePopupImage(),
                PlacementTarget = _control,
                PlacementMode = PlacementMode.Bottom
            };

            return popup;
        }

        private Image CreatePopupImage()
        {
            var temp = new Image();

            // TODO avalonia port
            // RenderOptions.SetBitmapScalingMode(temp, BitmapScalingMode.NearestNeighbor);

            temp.Stretch = Stretch.None;
            temp.HorizontalAlignment = HorizontalAlignment.Left;
            temp.VerticalAlignment = VerticalAlignment.Top;
            temp.Source = _popupImageBitmap;

            return temp;
        }

        private void Paint(WriteableBitmap bitmap, IntPtr sourceBuffer, int browserWidth, int browserHeight, CefRectangle[] dirtyRects)
        {
            int stride = browserWidth * 4;
            int sourceBufferSize = stride * browserHeight;

            _logger.Debug("Paint() Bitmap H{0}xW{1}, Browser H{2}xW{3}", bitmap.Size.Height, bitmap.Size.Width, browserHeight, browserWidth);

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            // TODO avalonia port - render only dirty regions
            // bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, (int)dirtyRect.X, (int)dirtyRect.Y);
            using (var l = bitmap.Lock())
            {
                byte[] managedArray = new byte[sourceBufferSize];

                Marshal.Copy(sourceBuffer, managedArray, 0, sourceBufferSize);
                Marshal.Copy(managedArray, 0, l.Address, sourceBufferSize);
            }
        }
    }
}
