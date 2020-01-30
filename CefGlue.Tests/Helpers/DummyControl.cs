using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;

namespace CefGlue.Tests.Helpers
{
    internal class DummyControl : IOffScreenControlHost
    {
        public OffScreenRenderSurface RenderSurface => throw new NotImplementedException();

        private class DummyRenderHandler : OffScreenRenderSurface
        {
            public override bool AllowsTransparency => throw new NotImplementedException();

            protected override int BytesPerPixel => throw new NotImplementedException();

            protected override int RenderedWidth => throw new NotImplementedException();

            protected override int RenderedHeight => throw new NotImplementedException();

            protected override Action BeginBitmapUpdate()
            {
                throw new NotImplementedException();
            }

            protected override void CreateBitmap(int width, int height)
            {
                throw new NotImplementedException();
            }

            protected override Task ExecuteInUIThread(Action action)
            {
                throw new NotImplementedException();
            }

            protected override void UpdateBitmap(IntPtr sourceBuffer, int sourceBufferSize, int stride, CefRectangle updateRegion)
            {
                throw new NotImplementedException();
            }
        }

        public event Action<CefSize> SizeChanged;
        public event Action GotFocus;
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

        public void Focus()
        {
            throw new NotImplementedException();
        }

        public IntPtr? GetHostWindowHandle()
        {
            return IntPtr.Zero;
        }

        public CefPoint PointToScreen(CefPoint point, float deviceScaleFactor)
        {
            throw new NotImplementedException();
        }

        public void SetCursor(IntPtr cursorHandle)
        {
            throw new NotImplementedException();
        }

        public void SetTooltip(string text)
        {
            throw new NotImplementedException();
        }

        public void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback)
        {
            throw new NotImplementedException();
        }

        public void CloseContextMenu()
        {
            throw new NotImplementedException();
        }

        public OffScreenRenderSurface CreateRenderHandler()
        {
            throw new NotImplementedException();
        }

        public Task<CefDragOperationsMask> StartDragging(CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void UpdateDragCursor(CefDragOperationsMask allowedOps)
        {
            throw new NotImplementedException();
        }

        public IntPtr? GetHostViewHandle()
        {
            throw new NotImplementedException();
        }

        public void InitializeRender(IntPtr browserHandle)
        {
            throw new NotImplementedException();
        }

        public Task<CefDragOperationsMask> StartDrag(CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            throw new NotImplementedException();
        }

        public void TriggerInitialization()
        {
            SizeChanged.Invoke(new CefSize(100,100));
        }
    }
}
