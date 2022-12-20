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
    internal unsafe struct cef_permission_handler_t
    {
        internal cef_base_ref_counted_t _base;
        internal IntPtr _on_request_media_access_permission;
        internal IntPtr _on_show_permission_prompt;
        internal IntPtr _on_dismiss_permission_prompt;
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate void add_ref_delegate(cef_permission_handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int release_delegate(cef_permission_handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int has_one_ref_delegate(cef_permission_handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int has_at_least_one_ref_delegate(cef_permission_handler_t* self);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int on_request_media_access_permission_delegate(cef_permission_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_string_t* requesting_origin, uint requested_permissions, cef_media_access_callback_t* callback);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate int on_show_permission_prompt_delegate(cef_permission_handler_t* self, cef_browser_t* browser, ulong prompt_id, cef_string_t* requesting_origin, uint requested_permissions, cef_permission_prompt_callback_t* callback);
        
        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
        #if !DEBUG
        [SuppressUnmanagedCodeSecurity]
        #endif
        internal delegate void on_dismiss_permission_prompt_delegate(cef_permission_handler_t* self, cef_browser_t* browser, ulong prompt_id, CefPermissionRequestResult result);
        
        private static int _sizeof;
        
        static cef_permission_handler_t()
        {
            _sizeof = Marshal.SizeOf(typeof(cef_permission_handler_t));
        }
        
        internal static cef_permission_handler_t* Alloc()
        {
            var ptr = (cef_permission_handler_t*)Marshal.AllocHGlobal(_sizeof);
            *ptr = new cef_permission_handler_t();
            ptr->_base._size = (UIntPtr)_sizeof;
            return ptr;
        }
        
        internal static void Free(cef_permission_handler_t* ptr)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
        
    }
}
