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
            return new CefCookie
                {
                    Name = cef_string_t.ToString(&ptr->name),
                    Value = cef_string_t.ToString(&ptr->value),
                    Domain = cef_string_t.ToString(&ptr->domain),
                    Path = cef_string_t.ToString(&ptr->path),
                    Secure = ptr->secure,
                    HttpOnly = ptr->httponly,
                    Creation = cef_time_t.ToDateTime(&ptr->creation),
                    LastAccess = cef_time_t.ToDateTime(&ptr->last_access),
                    Expires = ptr->has_expires ? (DateTime?)cef_time_t.ToDateTime(&ptr->expires) : null,
                };
        }

        internal cef_cookie_t* ToNative()
        {
            var ptr = cef_cookie_t.Alloc();

            cef_string_t.Copy(Name, &ptr->name);
            cef_string_t.Copy(Value, &ptr->value);
            cef_string_t.Copy(Domain, &ptr->domain);
            cef_string_t.Copy(Path, &ptr->path);
            ptr->secure = Secure;
            ptr->httponly = HttpOnly;
            ptr->creation = new cef_time_t(Creation);
            ptr->last_access = new cef_time_t(LastAccess);
            ptr->has_expires = Expires != null;
            ptr->expires = Expires != null ? new cef_time_t(Expires.Value) : new cef_time_t();

            return ptr;
        }

        internal static void Free(cef_cookie_t* ptr)
        {
            cef_cookie_t.Clear((cef_cookie_t*)ptr);
            cef_cookie_t.Free((cef_cookie_t*)ptr);
        }
    }
}
