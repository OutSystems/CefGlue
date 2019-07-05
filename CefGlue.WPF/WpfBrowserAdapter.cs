using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.WPF
{
    internal class WpfBrowserAdapter : CommonBrowserAdapter
    {
        private readonly ContentControl _control;
        private ToolTip _tooltip;
        private DispatcherTimer _tooltipTimer;
        private Dispatcher _mainUiDispatcher;

        private Image _browserImage;

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupBitmap;

        public WpfBrowserAdapter(ContentControl control, ILogger logger) : base(logger)
        {
            _control = control;
            _popup = CreatePopup();

            _tooltip = new ToolTip();
            _tooltip.StaysOpen = true;
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Closed += OnTooltipClosed;

            _tooltipTimer = new DispatcherTimer();
            _tooltipTimer.Interval = TimeSpan.FromSeconds(0.5);

            KeyboardNavigation.SetAcceptsReturn(_control, true);
            _mainUiDispatcher = Dispatcher.CurrentDispatcher;
        }

        protected override string Name => nameof(WpfBrowserAdapter);

        protected override object EventsEmitter => throw new NotImplementedException();

        protected override int RenderedWidth => throw new NotImplementedException();

        protected override int RenderedHeight => throw new NotImplementedException();

        protected override bool IsFocused => throw new NotImplementedException();

        protected override void AttachEventHandlers()
        {
            AttachEventHandlers(_control);
            AttachEventHandlers(_popup);
        }

        protected override IntPtr? GetHostWindowHandle()
        {
            var window = Window.GetWindow(_control);
            if (window != null)
            {
                return new WindowInteropHelper(window).Handle;
            }

            return null;
        }

        protected override void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            var ptScreen = new Point();

            _mainUiDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                WithErrorHandling(nameof(GetScreenPoint), () =>
                {
                    var ptView = new Point(viewX, viewY);
                    ptScreen = _control.PointToScreen(ptView);
                });
            }));

            screenX = (int)ptScreen.X;
            screenY = (int)ptScreen.Y;
        }

        protected override void OnBrowserSizeChanged(int newWidth, int newHeight)
        {
            throw new NotImplementedException();
        }

        protected override void OnCursorChanged(IntPtr cursorHandle)
        {
            throw new NotImplementedException();
        }

        protected override void OnPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects, bool isPopup)
        {
            throw new NotImplementedException();
        }

        protected override void OnPopupShow(bool show)
        {
            if (_popup == null)
            {
                return;
            }

            _mainUiDispatcher.Invoke(new Action(() => _popup.IsOpen = show));
        }

        protected override void OnPopupSizeChanged(CefRectangle rect)
        {
            _mainUiDispatcher.Invoke(
                new Action(
                    () =>
                    {
                        _popupBitmap = null;
                        _popupBitmap = new WriteableBitmap(rect.Width, rect.Height, 96, 96, PixelFormats.Bgr32, null);

                        _popupImage.Source = _popupBitmap;

                        _popup.Width = rect.Width;
                        _popup.Height = rect.Height;
                        _popup.HorizontalOffset = rect.X;
                        _popup.VerticalOffset = rect.Y;
                    }));
        }

        protected override bool OnTooltipChanged(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _tooltipTimer.Stop();
                UpdateTooltip(null);
            }
            else
            {
                _tooltipTimer.Tick += (sender, args) => UpdateTooltip(text);
                _tooltipTimer.Start();
            }
            return true;
        }

        private void AttachEventHandlers(UIElement target)
        {
            target.GotKeyboardFocus += (sender, arg) => HandleGotFocus();
            target.LostKeyboardFocus += (sender, arg) => HandleLostFocus();

            target.MouseMove += (sender, arg) => HandleMouseMove(arg.AsCefMouseEvent());
            target.MouseLeave += (sender, arg) => HandleMouseLeave(arg.AsCefMouseEvent());
            target.MouseDown += (sender, arg) =>
            {
                _control.Focus();
                HandleMouseButtonDown(arg.AsCefMouseEvent(), arg.ChangedButton.AsCefMouseButtonType(), arg.ClickCount);
            };
            target.MouseUp += (sender, arg) => HandleMouseButtonUp(arg.AsCefMouseEvent(), arg.ChangedButton.AsCefMouseButtonType());
            target.MouseWheel += (sender, arg) => HandleMouseWheel(arg.AsCefMouseEvent(), 0, arg.Delta);

            target.PreviewKeyDown += (sender, arg) => HandleKeyPress(arg, false);
            target.PreviewKeyUp += (sender, arg) => HandleKeyPress(arg, true);

            target.PreviewTextInput += (text) => HandleTextInput(text, out var handled);
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

        private void UpdateTooltip(string text)
        {
            _mainUiDispatcher.Invoke(
                DispatcherPriority.Render,
                new Action(
                    () =>
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            _tooltip.IsOpen = false;
                        }
                        else
                        {
                            _tooltip.Placement = PlacementMode.Mouse;
                            _tooltip.Content = text;
                            _tooltip.IsOpen = true;
                            _tooltip.Visibility = Visibility.Visible;
                        }
                    }));

            _tooltipTimer.Stop();
        }

        private void OnTooltipClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Placement = PlacementMode.Absolute;
        }

        private Popup CreatePopup()
        {
            _popupImage = CreatePopupImage();

            return new Popup
            {
                Child = _popupImage,
                PlacementTarget = _control,
                Placement = PlacementMode.Relative
            };
        }

        private Image CreatePopupImage()
        {
            var image = new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Source = _popupBitmap
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

            return image;
        }
    }
}