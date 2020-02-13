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
        public event Action GotFocus { add { } remove { } }
        public event Action LostFocus { add { } remove { } }
        public event KeyEventHandler KeyDown { add { } remove { } }
        public event KeyEventHandler KeyUp { add { } remove { } }
        public event TextInputEventHandler TextInput { add { } remove { } }
        public event Action<IOffScreenControlHost, CefMouseEvent, CefMouseButtonType, int> MouseButtonPressed { add { } remove { } }
        public event Action<CefMouseEvent, CefMouseButtonType> MouseButtonReleased { add { } remove { } }
        public event Action<CefMouseEvent> MouseLeave { add { } remove { } }
        public event Action<CefMouseEvent> MouseMoved { add { } remove { } }
        public event Action<CefMouseEvent, int, int> MouseWheelChanged { add { } remove { } }
        public event Action<CefMouseEvent, CefDragData, CefDragOperationsMask> DragEnter { add { } remove { } }
        public event Action<CefMouseEvent, CefDragOperationsMask> DragOver { add { } remove { } }
        public event Action DragLeave { add { } remove { } }
        public event Action<CefMouseEvent, CefDragOperationsMask> Drop { add { } remove { } }
        public event Action<float> ScreenInfoChanged { add { } remove { } }
        public event Action<bool> VisibilityChanged { add { } remove { } }

        public void Dispose() { }

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
