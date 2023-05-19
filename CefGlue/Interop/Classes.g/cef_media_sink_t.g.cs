//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Security;
    
    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
    internal unsafe struct cef_media_sink_t
    {
        internal cef_base_ref_counted_t _base;
        internal IntPtr _get_id;
        internal IntPtr _get_name;
        internal IntPtr _get_icon_type;
        internal IntPtr _get_device_info;
        internal IntPtr _is_cast_sink;
        internal IntPtr _is_dial_sink;
        internal IntPtr _is_compatible_with;
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate void add_ref_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int release_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int has_one_ref_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int has_at_least_one_ref_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_id_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate cef_string_userfree* get_name_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate CefMediaSinkIconType get_icon_type_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate void get_device_info_delegate(cef_media_sink_t* self, cef_media_sink_device_info_callback_t* callback);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int is_cast_sink_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int is_dial_sink_delegate(cef_media_sink_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        private delegate int is_compatible_with_delegate(cef_media_sink_t* self, cef_media_source_t* source);
        
        // AddRef
        private static IntPtr _p0;
        private static add_ref_delegate _d0;
        
        public static void add_ref(cef_media_sink_t* self)
        {
            add_ref_delegate d;
            var p = self->_base._add_ref;
            if (p == _p0) { d = _d0; }
            else
            {
                d = (add_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(add_ref_delegate));
                if (_p0 == IntPtr.Zero) { _d0 = d; _p0 = p; }
            }
            d(self);
        }
        
        // Release
        private static IntPtr _p1;
        private static release_delegate _d1;
        
        public static int release(cef_media_sink_t* self)
        {
            release_delegate d;
            var p = self->_base._release;
            if (p == _p1) { d = _d1; }
            else
            {
                d = (release_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(release_delegate));
                if (_p1 == IntPtr.Zero) { _d1 = d; _p1 = p; }
            }
            return d(self);
        }
        
        // HasOneRef
        private static IntPtr _p2;
        private static has_one_ref_delegate _d2;
        
        public static int has_one_ref(cef_media_sink_t* self)
        {
            has_one_ref_delegate d;
            var p = self->_base._has_one_ref;
            if (p == _p2) { d = _d2; }
            else
            {
                d = (has_one_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(has_one_ref_delegate));
                if (_p2 == IntPtr.Zero) { _d2 = d; _p2 = p; }
            }
            return d(self);
        }
        
        // HasAtLeastOneRef
        private static IntPtr _p3;
        private static has_at_least_one_ref_delegate _d3;
        
        public static int has_at_least_one_ref(cef_media_sink_t* self)
        {
            has_at_least_one_ref_delegate d;
            var p = self->_base._has_at_least_one_ref;
            if (p == _p3) { d = _d3; }
            else
            {
                d = (has_at_least_one_ref_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(has_at_least_one_ref_delegate));
                if (_p3 == IntPtr.Zero) { _d3 = d; _p3 = p; }
            }
            return d(self);
        }
        
        // GetId
        private static IntPtr _p4;
        private static get_id_delegate _d4;
        
        public static cef_string_userfree* get_id(cef_media_sink_t* self)
        {
            get_id_delegate d;
            var p = self->_get_id;
            if (p == _p4) { d = _d4; }
            else
            {
                d = (get_id_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_id_delegate));
                if (_p4 == IntPtr.Zero) { _d4 = d; _p4 = p; }
            }
            return d(self);
        }
        
        // GetName
        private static IntPtr _p5;
        private static get_name_delegate _d5;
        
        public static cef_string_userfree* get_name(cef_media_sink_t* self)
        {
            get_name_delegate d;
            var p = self->_get_name;
            if (p == _p5) { d = _d5; }
            else
            {
                d = (get_name_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_name_delegate));
                if (_p5 == IntPtr.Zero) { _d5 = d; _p5 = p; }
            }
            return d(self);
        }
        
        // GetIconType
        private static IntPtr _p6;
        private static get_icon_type_delegate _d6;
        
        public static CefMediaSinkIconType get_icon_type(cef_media_sink_t* self)
        {
            get_icon_type_delegate d;
            var p = self->_get_icon_type;
            if (p == _p6) { d = _d6; }
            else
            {
                d = (get_icon_type_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_icon_type_delegate));
                if (_p6 == IntPtr.Zero) { _d6 = d; _p6 = p; }
            }
            return d(self);
        }
        
        // GetDeviceInfo
        private static IntPtr _p7;
        private static get_device_info_delegate _d7;
        
        public static void get_device_info(cef_media_sink_t* self, cef_media_sink_device_info_callback_t* callback)
        {
            get_device_info_delegate d;
            var p = self->_get_device_info;
            if (p == _p7) { d = _d7; }
            else
            {
                d = (get_device_info_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(get_device_info_delegate));
                if (_p7 == IntPtr.Zero) { _d7 = d; _p7 = p; }
            }
            d(self, callback);
        }
        
        // IsCastSink
        private static IntPtr _p8;
        private static is_cast_sink_delegate _d8;
        
        public static int is_cast_sink(cef_media_sink_t* self)
        {
            is_cast_sink_delegate d;
            var p = self->_is_cast_sink;
            if (p == _p8) { d = _d8; }
            else
            {
                d = (is_cast_sink_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(is_cast_sink_delegate));
                if (_p8 == IntPtr.Zero) { _d8 = d; _p8 = p; }
            }
            return d(self);
        }
        
        // IsDialSink
        private static IntPtr _p9;
        private static is_dial_sink_delegate _d9;
        
        public static int is_dial_sink(cef_media_sink_t* self)
        {
            is_dial_sink_delegate d;
            var p = self->_is_dial_sink;
            if (p == _p9) { d = _d9; }
            else
            {
                d = (is_dial_sink_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(is_dial_sink_delegate));
                if (_p9 == IntPtr.Zero) { _d9 = d; _p9 = p; }
            }
            return d(self);
        }
        
        // IsCompatibleWith
        private static IntPtr _pa;
        private static is_compatible_with_delegate _da;
        
        public static int is_compatible_with(cef_media_sink_t* self, cef_media_source_t* source)
        {
            is_compatible_with_delegate d;
            var p = self->_is_compatible_with;
            if (p == _pa) { d = _da; }
            else
            {
                d = (is_compatible_with_delegate)Marshal.GetDelegateForFunctionPointer(p, typeof(is_compatible_with_delegate));
                if (_pa == IntPtr.Zero) { _da = d; _pa = p; }
            }
            return d(self, source);
        }
        
    }
}
