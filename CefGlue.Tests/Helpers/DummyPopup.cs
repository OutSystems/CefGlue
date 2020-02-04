using System;
using Xilium.CefGlue.Common.Platform;

namespace CefGlue.Tests.Helpers
{
    internal class DummyPopup : DummyControl, IOffScreenPopupHost
    {
        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public int OffsetX => throw new NotImplementedException();

        public int OffsetY => throw new NotImplementedException();

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void MoveAndResize(int x, int y, int width, int height)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }
    }
}
