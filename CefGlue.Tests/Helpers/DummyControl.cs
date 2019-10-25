using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;

namespace CefGlue.Tests.Helpers
{
    internal class DummyControl : UIControl
    {
        private class DummyRenderHandler : BuiltInRenderHandler
        {
            public DummyRenderHandler() : base(new DummyLogger())
            {
            }

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

        public DummyControl() : base(new DummyRenderHandler())
        {
        }

        public override void Focus()
        {
            throw new NotImplementedException();
        }

        public override IntPtr? GetHostWindowHandle()
        {
            return IntPtr.Zero;
        }

        public override Point PointToScreen(Point point)
        {
            throw new NotImplementedException();
        }

        public override void SetCursor(IntPtr cursorHandle)
        {
            throw new NotImplementedException();
        }

        public override void SetTooltip(string text)
        {
            throw new NotImplementedException();
        }

        public override void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback)
        {
            throw new NotImplementedException();
        }

        public override void CloseContextMenu()
        {
            throw new NotImplementedException();
        }
    }
}
