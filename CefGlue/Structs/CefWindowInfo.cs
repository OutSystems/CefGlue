namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue.Interop;

    public sealed unsafe class CefWindowInfo
    {
        private readonly cef_window_info_t* _self;
        private bool _disposed;

        private IntPtr _parentWindowHandle;

        public CefWindowInfo()
        {
            _self = null;
        }

        internal CefWindowInfo(cef_window_info_t* ptr)
        {
            if (ptr == null) throw new ArgumentNullException("ptr");

            _self = ptr;
        }

        internal void Dispose()
        {
            _disposed = true;
        }

        private void ThrowIfObjectDisposed()
        {
            if (_disposed) throw ExceptionBuilder.ObjectDisposed();
        }

        public IntPtr Parent
        {
            get
            {
                ThrowIfObjectDisposed();

                if (_self == null)
                    return _parentWindowHandle;

                switch (CefRuntime.Platform)
                {
                    case CefRuntimePlatform.Windows:
                        {
                            var self = (cef_window_info_t_windows*)_self;
                            return self->parent_window;
                        }

                    case CefRuntimePlatform.Linux:
                        {
                            var self = (cef_window_info_t_linux*)_self;
                            return self->parent_widget;
                        }

                    case CefRuntimePlatform.MacOSX:
                        {
                            var self = (cef_window_info_t_mac*)_self;
                            return self->parent_view;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
            set
            {
                ThrowIfObjectDisposed();

                if (_self == null)
                {
                    _parentWindowHandle = value;
                    return;
                }

                switch (CefRuntime.Platform)
                {
                    case CefRuntimePlatform.Windows:
                        {
                            var self = (cef_window_info_t_windows*)_self;
                            self->parent_window = value;
                            return;
                        }

                    case CefRuntimePlatform.Linux:
                        {
                            var self = (cef_window_info_t_linux*)_self;
                            self->parent_widget = value;
                            return;
                        }

                    case CefRuntimePlatform.MacOSX:
                        {
                            var self = (cef_window_info_t_mac*)_self;
                            self->parent_view = value;
                            return;
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        internal cef_window_info_t* ToNative()
        {
            var ptr = cef_window_info_t_windows.Alloc();

            ptr->style = (uint)(WindowStyles.WS_CHILD
                | WindowStyles.WS_CLIPCHILDREN
                | WindowStyles.WS_CLIPSIBLINGS
                | WindowStyles.WS_TABSTOP
                | WindowStyles.WS_VISIBLE);

            ptr->parent_window = Parent;
            ptr->x = 0;
            ptr->y = 0;
            ptr->width = 400;
            ptr->height = 600;

            return (cef_window_info_t*)ptr;
        }

        /*
  void SetAsChild(HWND hWndParent, RECT windowRect) {
    style = WS_CHILD | WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_TABSTOP |
            WS_VISIBLE;
    parent_window = hWndParent;
    x = windowRect.left;
    y = windowRect.top;
    width = windowRect.right - windowRect.left;
    height = windowRect.bottom - windowRect.top;
  }

  void SetAsPopup(HWND hWndParent, const CefString& windowName) {
    style = WS_OVERLAPPEDWINDOW | WS_CLIPCHILDREN | WS_CLIPSIBLINGS |
            WS_VISIBLE;
    parent_window = hWndParent;
    x = CW_USEDEFAULT;
    y = CW_USEDEFAULT;
    width = CW_USEDEFAULT;
    height = CW_USEDEFAULT;

    cef_string_copy(windowName.c_str(), windowName.length(), &window_name);
  }

  void SetTransparentPainting(BOOL transparentPainting) {
    transparent_painting = transparentPainting;
  }
*/
    }
}
