namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue.Interop;

    public sealed unsafe class CefCookie
    {
        public CefCookie()
        { }

        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public DateTime Creation { get; set; }
        public DateTime LastAccess { get; set; }
        public DateTime? Expires { get; set; }

        internal static CefCookie FromNative(cef_cookie_t* ptr)
        {
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                case CefRuntimePlatform.Linux:
                    return FromNativeOther((cef_cookie_t_other*)ptr);

                case CefRuntimePlatform.MacOSX:
                    return FromNativeMac((cef_cookie_t_mac*)ptr);

                default:
                    throw ExceptionBuilder.UnsupportedPlatform();
            }
        }

        private static CefCookie FromNativeOther(cef_cookie_t_other* ptr)
        {
            return new CefCookie()
                {
                    Name = cef_string_t.ToString(&ptr->name),
                    Value = cef_string_t.ToString(&ptr->value),
                    Domain = cef_string_t.ToString(&ptr->value),
                    Path = cef_string_t.ToString(&ptr->value),
                    Secure = ptr->secure,
                    HttpOnly = ptr->httponly,
                    Creation = cef_time_t_other.ToDateTime(&ptr->creation),
                    LastAccess = cef_time_t_other.ToDateTime(&ptr->last_access),
                    Expires = ptr->has_expires ? (DateTime?)cef_time_t_other.ToDateTime(&ptr->expires) : null
                };
        }

        private static CefCookie FromNativeMac(cef_cookie_t_mac* ptr)
        {
            return new CefCookie()
            {
                Name = cef_string_t.ToString(&ptr->name),
                Value = cef_string_t.ToString(&ptr->value),
                Domain = cef_string_t.ToString(&ptr->value),
                Path = cef_string_t.ToString(&ptr->value),
                Secure = ptr->secure,
                HttpOnly = ptr->httponly,
                Creation = cef_time_t_mac.ToDateTime(&ptr->creation),
                LastAccess = cef_time_t_mac.ToDateTime(&ptr->last_access),
                Expires = ptr->has_expires ? (DateTime?)cef_time_t_mac.ToDateTime(&ptr->expires) : null
            };
        }

        internal cef_cookie_t* ToNative()
        {
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                case CefRuntimePlatform.Linux:
                    return (cef_cookie_t*)ToNativeOther();

                case CefRuntimePlatform.MacOSX:
                    return (cef_cookie_t*)ToNativeMac();

                default:
                    throw ExceptionBuilder.UnsupportedPlatform();
            }
        }

        private cef_cookie_t_other* ToNativeOther()
        {
            var ptr = cef_cookie_t_other.Alloc();

            cef_string_t.Copy(Name, &ptr->name);
            cef_string_t.Copy(Value, &ptr->value);
            cef_string_t.Copy(Domain, &ptr->domain);
            cef_string_t.Copy(Path, &ptr->path);
            ptr->secure = Secure;
            ptr->httponly = HttpOnly;
            ptr->creation = new cef_time_t_other(Creation);
            ptr->last_access = new cef_time_t_other(LastAccess);
            ptr->has_expires = Expires != null;
            ptr->expires = Expires != null ? new cef_time_t_other(Expires.Value) : new cef_time_t_other();

            return ptr;
        }

        private cef_cookie_t_mac* ToNativeMac()
        {
            var ptr = cef_cookie_t_mac.Alloc();

            cef_string_t.Copy(Name, &ptr->name);
            cef_string_t.Copy(Value, &ptr->value);
            cef_string_t.Copy(Domain, &ptr->domain);
            cef_string_t.Copy(Path, &ptr->path);
            ptr->secure = Secure;
            ptr->httponly = HttpOnly;
            ptr->creation = new cef_time_t_mac(Creation);
            ptr->last_access = new cef_time_t_mac(LastAccess);
            ptr->has_expires = Expires != null;
            ptr->expires = Expires != null ? new cef_time_t_mac(Expires.Value) : new cef_time_t_mac();

            return ptr;
        }

        internal static void Free(cef_cookie_t* ptr)
        {
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                case CefRuntimePlatform.Linux:
                    cef_cookie_t_other.Clear((cef_cookie_t_other*)ptr);
                    cef_cookie_t_other.Free((cef_cookie_t_other*)ptr);
                    return;

                case CefRuntimePlatform.MacOSX:
                    cef_cookie_t_mac.Clear((cef_cookie_t_mac*)ptr);
                    cef_cookie_t_mac.Free((cef_cookie_t_mac*)ptr);
                    return;

                default:
                    throw ExceptionBuilder.UnsupportedPlatform();
            }
        }
    }
}
