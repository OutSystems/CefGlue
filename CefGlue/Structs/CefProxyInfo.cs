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

        /// <summary>
        /// Use a direct connection instead of a proxy.
        /// </summary>
        public void UseDirect()
        {
            ProxyType = CefProxyType.Direct;
        }

        /// <summary>
        /// Use one or more named proxy servers specified in WinHTTP format. Each proxy
        /// server is of the form:
        ///
        /// [scheme"://"]server[":"port]
        ///
        /// Multiple values may be separated by semicolons or whitespace. For example,
        /// "foo1:80;foo2:80".
        /// </summary>
        public void UseNamedProxy(string proxyUriList)
        {
            ProxyType = CefProxyType.Named;
            List = proxyUriList;
        }

        /// <summary>
        /// Use one or more named proxy servers specified in PAC script format. For
        /// example, "PROXY foobar:99; SOCKS fml:2; DIRECT".
        /// </summary>
        public void UsePacString(string pacString)
        {
            ProxyType = CefProxyType.PacString;
            List = pacString;
        }
    }
}
