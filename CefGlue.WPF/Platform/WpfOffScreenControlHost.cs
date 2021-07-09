using Microsoft.Win32.SafeHandles;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.WPF.Platform
{
    /// <summary>
    /// The WPF control wrapper.
    /// </summary>
    internal class WpfOffScreenControlHost : WpfControl, IOffScreenControlHost
    {
        private ToolTip _tooltip;
        private DispatcherTimer _tooltipTimer;

        private Point _browserScreenLocation;

        public event Action LostFocus;
        public event Common.Platform.KeyEventHandler KeyDown;
        public event Common.Platform.KeyEventHandler KeyUp;
        public event TextInputEventHandler TextInput;
        public event Action<IOffScreenControlHost, CefMouseEvent, CefMouseButtonType, int> MouseButtonPressed;
        public event Action<CefMouseEvent, CefMouseButtonType> MouseButtonReleased;
        public event Action<CefMouseEvent> MouseLeave;
        public event Action<CefMouseEvent> MouseMoved;
        public event Action<CefMouseEvent, int, int> MouseWheelChanged;
        public event Action<CefMouseEvent, CefDragData, CefDragOperationsMask> DragEnter;
        public event Action<CefMouseEvent, CefDragOperationsMask> DragOver;
        public event Action DragLeave;
        public event Action<CefMouseEvent, CefDragOperationsMask> Drop;
        public event Action<float> ScreenInfoChanged;
        public event Action<bool> VisibilityChanged;

        public WpfOffScreenControlHost(FrameworkElement control) : base(control)
        {
            control.AllowDrop = true;

            control.IsVisibleChanged += OnIsVisibleChanged;
            control.LostKeyboardFocus += OnLostFocus;

            control.MouseMove += OnMouseMove;
            control.MouseLeave += OnMouseLeave;
            control.MouseDown += OnMouseDown;
            control.MouseUp += OnMouseUp;
            control.MouseWheel += OnMouseWheel;

            control.DragEnter += OnDragEnter;
            control.DragOver += OnDragOver;
            control.DragLeave += OnDragLeave;
            control.Drop += OnDrop;

            control.Loaded += OnLoaded;
            control.Unloaded += OnUnloaded;

            control.KeyDown += OnKeyDown;
            control.KeyUp += OnKeyUp;

            control.TextInput += OnTextInput;

            _tooltip = new ToolTip();
            _tooltip.StaysOpen = true;
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Closed += OnTooltipClosed;

            _tooltipTimer = new DispatcherTimer();
            _tooltipTimer.Interval = TimeSpan.FromSeconds(0.5);

            var image = CreateImage();
            SetContent(image);
            RenderSurface = new WpfRenderSurface(image);
        }

        public OffScreenRenderSurface RenderSurface { get; }

        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            var handled = false;
            TextInput?.Invoke(e.Text, out handled);
            e.Handled = handled;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            var handled = false;
            KeyUp?.Invoke(e.AsCefKeyEvent(true), out handled);
            e.Handled = handled;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var handled = false;
            KeyDown?.Invoke(e.AsCefKeyEvent(false), out handled);

            var key = e.Key;
            if (key == Key.Tab  // Avoid tabbing out the web browser control
                || key == Key.Home || key == Key.End // Prevent keyboard navigation using home and end keys
                || key == Key.Up || key == Key.Down || key == Key.Left || key == Key.Right // Prevent keyboard navigation using arrows
            )
            {
                handled = true;
            }

            e.Handled = handled;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            Drop?.Invoke(e.AsCefMouseEvent(MousePositionReferential), e.AllowedEffects.AsCefDragOperationsMask());
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            DragLeave?.Invoke();
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            DragOver?.Invoke(e.AsCefMouseEvent(MousePositionReferential), e.AllowedEffects.AsCefDragOperationsMask());
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            DragEnter?.Invoke(e.AsCefMouseEvent(MousePositionReferential), e.GetDragData(), e.AllowedEffects.AsCefDragOperationsMask());
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MouseWheelChanged?.Invoke(e.AsCefMouseEvent(MousePositionReferential), 0, (int)e.Delta);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseButtonReleased?.Invoke(e.AsCefMouseEvent(MousePositionReferential), e.ChangedButton.AsCefMouseButtonType());
            if (e.ChangedButton == MouseButton.Left)
            {
                Mouse.Capture(null);
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseButtonPressed?.Invoke(this, e.AsCefMouseEvent(MousePositionReferential), e.ChangedButton.AsCefMouseButtonType(), e.ClickCount);
            if (e.ChangedButton == MouseButton.Left)
            {
                Mouse.Capture(_control); // allow capturing mouse mouse when outside the webview (eg: grabbing scrollbar)
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave?.Invoke(e.AsCefMouseEvent(MousePositionReferential));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseMoved?.Invoke(e.AsCefMouseEvent(MousePositionReferential));
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            LostFocus?.Invoke();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            VisibilityChanged?.Invoke(_control.IsVisible);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            PresentationSource.AddSourceChangedHandler(element, OnPresentationSourceChanged);
            var source = PresentationSource.FromVisual(element);
            UpdatePresentationSource(source, source); // same parameters passed on purpose, to make sure the events don't get registered more than once to the same source
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            PresentationSource.RemoveSourceChangedHandler(element, OnPresentationSourceChanged);
            _tooltip.IsOpen = false;
            _tooltipTimer.Stop();
        }

        private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e)
        {
            UpdatePresentationSource(e.OldSource, e.NewSource);
        }

        private void UpdatePresentationSource(PresentationSource oldSource, PresentationSource newSource)
        {
            if (oldSource?.RootVisual is Window oldWindow)
            {
                oldWindow.StateChanged -= OnHostWindowStateChanged;
                oldWindow.LocationChanged -= OnHostWindowLocationChanged;
            }

            if (newSource != null)
            {
                _browserScreenLocation = GetBrowserScreenLocation();

                var matrix = newSource.CompositionTarget.TransformToDevice;
                ScreenInfoChanged?.Invoke((float)matrix.M11);

                if (newSource.RootVisual is Window window)
                {
                    window.StateChanged += OnHostWindowStateChanged;
                    window.LocationChanged += OnHostWindowLocationChanged;
                }
            }
        }

        protected virtual IInputElement MousePositionReferential => _control;

        public void Focus()
        {
            _control.Focus();
        }

        public CefPoint PointToScreen(CefPoint point, float deviceScaleFactor)
        {
            // calculate the point based on the browser stored location, 
            // since PointToScreen needs to be executed on the dispatcher thread
            // but calling Invoke at this stage can lead to dead locks
            return new CefPoint((int) (_browserScreenLocation.X + point.X * deviceScaleFactor), (int) (_browserScreenLocation.Y + point.Y * deviceScaleFactor));
        }

        public override bool SetCursor(IntPtr cursorHandle, CefCursorType cursorType)
        {
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    var cursor = CursorInteropHelper.Create(new SafeFileHandle(cursorHandle, false));
                    _control.Cursor = cursor;
                }));

            return true;
        }

        public override void SetTooltip(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                UpdateTooltip(null);
            }
            else
            {
                _tooltipTimer.Tick += (sender, args) => UpdateTooltip(text);
                _tooltipTimer.Start();
            }
        }

        public async Task<CefDragOperationsMask> StartDrag(CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            var dataObject = new DataObject();

            dataObject.SetText(dragData.FragmentText ?? "", TextDataFormat.Text);
            dataObject.SetText(dragData.FragmentText ?? "", TextDataFormat.UnicodeText);
            dataObject.SetText(dragData.FragmentHtml ?? "", TextDataFormat.Html);

            var result = DragDropEffects.None;

            await _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    result = DragDrop.DoDragDrop(_control, dataObject, allowedOps.AsDragDropEffects());
                })
            );

            return result.AsCefDragOperationsMask();
        }

        public void UpdateDragCursor(CefDragOperationsMask allowedOps)
        {
            // do nothing
        }

        private void UpdateTooltip(string text)
        {
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
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

        private void OnHostWindowLocationChanged(object sender, EventArgs e)
        {
            _browserScreenLocation = GetBrowserScreenLocation();
        }

        private void OnHostWindowStateChanged(object sender, EventArgs e)
        {
            var window = (Window)sender;

            switch (window.WindowState)
            {
                case WindowState.Normal:
                case WindowState.Maximized:
                    VisibilityChanged?.Invoke(_control.IsVisible);
                    break;

                case WindowState.Minimized:
                    VisibilityChanged?.Invoke(false);
                    break;
            }
        }

        private Point GetBrowserScreenLocation()
        {
            if (PresentationSource.FromVisual(_control) != null)
            {
                var wpfPoint = _control.PointToScreen(new Point());
                return new Point((int) wpfPoint.X, (int) wpfPoint.Y);
            }
            return new Point(0, 0);
        }

        protected virtual void SetContent(Image image)
        {
            ((ContentControl) _control).Content = image;
        }

        /// <summary>
        /// Create an image that is used to render the browser frame and popups
        /// </summary>
        /// <returns></returns>
        private static Image CreateImage()
        {
            var image = new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            return image;
        }
    }
}
