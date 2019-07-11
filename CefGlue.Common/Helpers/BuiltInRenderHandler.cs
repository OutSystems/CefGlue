using System;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Common.Helpers
{
    /// <summary>
    /// Builtin render handler that supports rendering into the browser contents into and image.
    /// </summary>
    internal abstract class BuiltInRenderHandler : IDisposable
    {
        protected readonly ILogger _logger;

        protected bool _sizeChanged;
        private int _width;
        private int _height;

        protected BuiltInRenderHandler(ILogger logger)
        {
            _logger = logger;
        }

        public int Width => _width;

        public int Height => _height;

        public event Action<Exception> ExceptionOcurred;

        public abstract void Dispose();

        public void Paint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            // When browser size changed - we just skip frame updating.
            // This is dirty precheck to do not do Invoke whenever is possible.
            if (_sizeChanged && (width != Width || height != Height))
                return;

            InnerPaint(buffer, width, height, dirtyRects);
        }

        protected abstract void InnerPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects);

        protected void HandleException(Exception e)
        {
            ExceptionOcurred?.Invoke(e);
        }

        public void Resize(int width, int height)
        {
            if (_width != width)
            {
                _width = width;
                _sizeChanged = true;
            }
            if (_height != height)
            {
                _height = height;
                _sizeChanged = true;
            }
        }
    }
}
