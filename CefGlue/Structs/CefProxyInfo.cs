namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue.Interop;

    public sealed unsafe class CefProxyInfo
    {
        private cef_proxy_info_t* _self;

        internal CefProxyInfo(cef_proxy_info_t* ptr)
        {
            _self = ptr;
        }

        internal void Dispose()
        {
            _self = null;
        }

        private void ThrowIfDisposed()
        {
            if (_self == null) throw ExceptionBuilder.ObjectDisposed();
        }

        public CefProxyType ProxyType
        {
            get
            {
                ThrowIfDisposed();
                return _self->proxyType;
            }
            set
            {
                ThrowIfDisposed();
                _self->proxyType = value;
            }
        }

        public string List
        {
            get { return cef_string_t.ToString(&_self->proxyList); }
            set { cef_string_t.Copy(value, &_self->proxyList); }
        }
    }
}
