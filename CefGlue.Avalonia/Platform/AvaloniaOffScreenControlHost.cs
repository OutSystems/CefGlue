using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Avalonia.Platform
{
    /// <summary>
    /// The Avalonia control wrapper.
    /// </summary>
    internal class AvaloniaOffScreenControlHost : AvaloniaControl, IOffScreenControlHost
    {
        // TODO avalonia: get value from OS
        private const int MouseWheelDelta = 100;

        private IDisposable _windowStateChangedObservable;

        private PointerPressedEventArgs _lastPointerEvent;
        private Cursor _currentDragCursor;
        private Cursor _previousCursor;

        public event Action LostFocus;
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
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

        public AvaloniaOffScreenControlHost(Control control, IAvaloniaList<Visual> visualChildren) :
            base(control, visualChildren)
        {
            DragDrop.SetAllowDrop(control, true);

            control.AttachedToVisualTree += OnAttachedToVisualTree;
            control.DetachedFromVisualTree += OnDetachedFromVisualTree;

            control.LostFocus += OnLostFocus;

            control.PointerMoved += OnPointerMoved;
            control.PointerExited += OnPointerExited;

            control.PointerPressed += OnPointerPressed;
            control.PointerReleased += OnPointerReleased;

            control.PointerWheelChanged += OnPointerWheelChanged;

            control.AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
            control.AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
            control.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            control.AddHandler(DragDrop.DropEvent, OnDrop);

            control.KeyDown += OnKeyDown;
            control.KeyUp += OnKeyUp;
            control.TextInput += OnTextInput;

            var image = CreateImage();
            var viewbox = new Viewbox()
            {
                Child = image,
                Stretch = Stretch.Fill,
                StretchDirection = StretchDirection.Both
            };
            SetContent(viewbox);
            RenderSurface = new AvaloniaRenderSurface(image);
        }

        public OffScreenRenderSurface RenderSurface { get; }

        private void OnTextInput(object sender, TextInputEventArgs e)
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
            _control.Cursor = _previousCursor; // restore cursor
            Drop?.Invoke(e.AsCefMouseEvent(MousePositionReferential), e.DragEffects.AsCefDragOperationsMask());
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            DragOver?.Invoke(e.AsCefMouseEvent(MousePositionReferential), e.DragEffects.AsCefDragOperationsMask());
            _control.Cursor = _currentDragCursor;
        }

        private void OnDragLeave(object sender, RoutedEventArgs e)
        {
            DragLeave?.Invoke();
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            _previousCursor = _control.Cursor;
            DragEnter?.Invoke(e.AsCefMouseEvent(MousePositionReferential), e.GetDragData(), e.DragEffects.AsCefDragOperationsMask());
        }

        private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            MouseWheelChanged?.Invoke(e.AsCefMouseEvent(MousePositionReferential), (int)e.Delta.X * MouseWheelDelta, (int)e.Delta.Y * MouseWheelDelta);
        }

        private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var button = e.AsCefMouseButtonType();
            MouseButtonReleased?.Invoke(e.AsCefMouseEvent(MousePositionReferential), button);
            if (button == CefMouseButtonType.Left)
            {
                e.Pointer.Capture(null);
            }
        }

        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            _lastPointerEvent = e;
            var button = e.AsCefMouseButtonType();
            MouseButtonPressed?.Invoke(this, e.AsCefMouseEvent(MousePositionReferential), button, e.ClickCount);
            if (button == CefMouseButtonType.Left)
            {
                e.Pointer.Capture(_control);
            }
        }

        private void OnPointerExited(object sender, PointerEventArgs e)
        {
            MouseLeave?.Invoke(e.AsCefMouseEvent(MousePositionReferential));
        }

        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
            MouseMoved?.Invoke(e.AsCefMouseEvent(MousePositionReferential));
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            LostFocus?.Invoke();
        }

        private void OnDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _lastPointerEvent = null;
            _previousCursor = null;
            _currentDragCursor = null;
            _windowStateChangedObservable?.Dispose();
            VisibilityChanged(false);
        }

        private void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            VisibilityChanged?.Invoke(true);
            if (e.Root is Window newWindow)
            {
                _windowStateChangedObservable = newWindow.GetPropertyChangedObservable(Window.WindowStateProperty).Subscribe(OnHostWindowStateChanged);
            }
            if (e.Root.RenderScaling != RenderSurface.DeviceScaleFactor)
            {
                RenderSurface.DeviceScaleFactor = (float)e.Root.RenderScaling;
                ScreenInfoChanged?.Invoke(RenderSurface.DeviceScaleFactor);
            }
        }

        private void OnHostWindowStateChanged(AvaloniaPropertyChangedEventArgs e)
        {
            switch ((WindowState)e.NewValue)
            {
                case WindowState.Normal:
                case WindowState.Maximized:
                    VisibilityChanged?.Invoke(_control.IsEffectivelyVisible);
                    break;

                case WindowState.Minimized:
                    VisibilityChanged?.Invoke(false);
                    break;
            }
        }

        protected virtual Visual MousePositionReferential => _control;

        public CefPoint PointToScreen(CefPoint point, float deviceScaleFactor)
        {
            var screenCoordinates = _control.PointToScreen(new Point(point.X, point.Y));

            var result = new CefPoint(0, 0);
            result.X = screenCoordinates.X;
            result.Y = screenCoordinates.Y;
            return result;
        }

        public override IntPtr? GetHostViewHandle(int initialWidth, int initialHeight)
        {
            var platformHandle = GetHostWindowPlatformHandle();
            if (platformHandle is IMacOSTopLevelPlatformHandle macOSHandle)
            {
                return macOSHandle.NSView;
            }
            return platformHandle?.Handle;
        }

        public override void SetTooltip(string text)
        {
            // TODO BUG: sometimes the tooltips are left hanging when the user moves the cursor over the tooltip but meanwhile
            // the tooltip is destroyed
            Dispatcher.UIThread.Post(
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

        public void Focus()
        {
            _control.Focus();
        }

        public async Task<CefDragOperationsMask> StartDrag(CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            var lastPointerEvent = this._lastPointerEvent; // story a copy, since this might be other thread
            if (lastPointerEvent != null)
            {
                var dataObject = new DataObject();
                dataObject.Set(DataFormats.Text, dragData.FragmentText);

                var result = await Dispatcher.UIThread.InvokeAsync(() => DragDrop.DoDragDrop(lastPointerEvent, dataObject, allowedOps.AsDragDropEffects()));
                this._lastPointerEvent = null;
                _previousCursor = null;
                _currentDragCursor = null;
                return result.AsCefDragOperationsMask();
            }
            return CefDragOperationsMask.None;
        }

        public void UpdateDragCursor(CefDragOperationsMask allowedOps)
        {
            _currentDragCursor = CursorsProvider.GetCursorFromCefType(allowedOps);
        }

        public override bool SetCursor(IntPtr cursorHandle, CefCursorType cursorType)
        {
            var cursor = CursorsProvider.GetCursorFromCefType(cursorType);
            Dispatcher.UIThread.Post(() => _control.Cursor = cursor);
            return true;
        }

        /// <summary>
        /// Create an image that is used to render the browser frame and popups
        /// </summary>
        /// <returns></returns>
        private static Image CreateImage()
        {
            return new Image()
            {
                Focusable = false,
                Stretch = Stretch.None,
                HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Left,
                VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Top,
            };
        }
    }
}
