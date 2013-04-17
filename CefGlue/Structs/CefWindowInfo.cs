namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue.Interop;
    using Xilium.CefGlue.Platform;
    using Xilium.CefGlue.Platform.Windows;

    /// <summary>
    /// Class representing window information.
    /// </summary>
    /// <remarks>
    /// Meanings for handles:
    /// <list type="table">
    ///     <listheader>
    ///         <item>Platform</item>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <item>Windows</item>
    ///         <description>Window handle (HWND)</description>
    ///     </item>
    ///     <item>
    ///         <item>Mac OS X</item>
    ///         <description>NSView pointer for the parent view (NSView*)</description>
    ///     </item>
    ///     <item>
    ///         <item>Linux</item>
    ///         <description>Pointer for the new browser widget (GtkWidget*)</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public unsafe abstract class CefWindowInfo
    {
        public static CefWindowInfo Create()
        {
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows: return new CefWindowInfoWindowsImpl();
                case CefRuntimePlatform.Linux: return new CefWindowInfoLinuxImpl();
                case CefRuntimePlatform.MacOSX: return new CefWindowInfoMacImpl();
                default: throw new NotSupportedException();
            }
        }

        internal static CefWindowInfo FromNative(cef_window_info_t* ptr)
        {
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows: return new CefWindowInfoWindowsImpl(ptr);
                case CefRuntimePlatform.Linux: return new CefWindowInfoLinuxImpl(ptr);
                case CefRuntimePlatform.MacOSX: return new CefWindowInfoMacImpl(ptr);
                default: throw new NotSupportedException();
            }
        }

        private bool _own;
        private bool _disposed;

        protected internal CefWindowInfo(bool own)
        {
            _own = own;
        }

        ~CefWindowInfo()
        {
            Dispose();
        }

        internal void Dispose()
        {
            _disposed = true;
            if (_own)
            {
                DisposeNativePointer();
            }
            GC.SuppressFinalize(this);
        }

        internal cef_window_info_t* ToNative()
        {
            var ptr = GetNativePointer();
            _own = false;
            return ptr;
        }

        protected internal void ThrowIfDisposed()
        {
            if (_disposed) throw ExceptionBuilder.ObjectDisposed();
        }

        public bool Disposed { get { return _disposed; } }

        internal abstract cef_window_info_t* GetNativePointer();
        protected internal abstract void DisposeNativePointer();

        // Common properties for all platforms
        /// <summary>
        /// Handle for the parent window.
        /// </summary>
        public abstract IntPtr ParentHandle { get; set; }

        /// <summary>
        /// Handle for the new browser window.
        /// </summary>
        public abstract IntPtr Handle { get; set; }

        // Common properties for windows & macosx
        public abstract string Name { get; set; }
        public abstract int X { get; set; }
        public abstract int Y { get; set; }
        public abstract int Width { get; set; }
        public abstract int Height { get; set; }

        // Windows-specific
        public abstract WindowStyle Style { get; set; }
        public abstract WindowStyleEx StyleEx { get; set; }
        public abstract IntPtr MenuHandle { get; set; }

        // Mac-specific
        public abstract bool Hidden { get; set; }

        /// <summary>
        /// -- Windows --
        /// If window rendering is disabled no browser window will be created. Set
        /// |parent_window| to be used for identifying monitor info
        /// (MonitorFromWindow). If |parent_window| is not provided the main screen
        /// monitor will be used.
        /// 
        /// -- MacOSX --
        /// If window rendering is disabled no browser window will be created. Set
        /// |parent_view| to the window that will act as the parent for popup menus,
        /// dialog boxes, etc.
        /// 
        /// -- Linux --
        /// Unsupported
        /// </summary>
        public abstract bool WindowRenderingDisabled { get; set; }

        /// <summary>
        /// Set to true to enable transparent painting.
        /// If window rendering is disabled and |transparent_painting| is set to true
        /// WebKit rendering will draw on a transparent background (RGBA=0x00000000).
        /// When this value is false the background will be white and opaque.
        /// </summary>
        public abstract bool TransparentPainting { get; set; }

        public void SetAsChild(IntPtr parentHandle, CefRectangle rect)
        {
            ThrowIfDisposed();

            Style = WindowStyle.WS_CHILD
                  | WindowStyle.WS_CLIPCHILDREN
                  | WindowStyle.WS_CLIPSIBLINGS
                  | WindowStyle.WS_TABSTOP
                  | WindowStyle.WS_VISIBLE;

            ParentHandle = parentHandle;

            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        public void SetAsPopup(IntPtr parentHandle, string name)
        {
            ThrowIfDisposed();

            Style = WindowStyle.WS_OVERLAPPEDWINDOW
                  | WindowStyle.WS_CLIPCHILDREN
                  | WindowStyle.WS_CLIPSIBLINGS
                  | WindowStyle.WS_VISIBLE;

            ParentHandle = parentHandle;

            // CW_USEDEFAULT
            X = int.MinValue;
            Y = int.MinValue;
            Width = int.MinValue;
            Height = int.MinValue;

            Name = name;
        }

        public void SetTransparentPainting(bool transparentPainting)
        {
            TransparentPainting = transparentPainting;
        }

        public void SetAsOffScreen(IntPtr parentHandle)
        {
            WindowRenderingDisabled = true;
            ParentHandle = parentHandle;
        }
    }
}
