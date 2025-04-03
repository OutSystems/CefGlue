namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Interface that should be implemented to handle V8 interceptor calls. The
    /// methods of this class will be called on the thread associated with the V8
    /// interceptor. Interceptor's named property handlers (with first argument of
    /// type CefString) are called when object is indexed by string. Indexed
    /// property handlers (with first argument of type int) are called when object
    /// is indexed by integer.
    /// </summary>
    public abstract unsafe partial class CefV8Interceptor
    {
        private int get_byname(cef_v8interceptor_t* self, cef_string_t* name, cef_v8value_t* @object, cef_v8value_t** retval, cef_string_t* exception)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefV8Interceptor.Get
        }
        
        /// <summary>
        /// Handle retrieval of the interceptor value identified by |name|. |object|
        /// is the receiver ('this' object) of the interceptor. If retrieval succeeds,
        /// set |retval| to the return value. If the requested value does not exist,
        /// don't set either |retval| or |exception|. If retrieval fails, set
        /// |exception| to the exception that will be thrown. If the property has an
        /// associated accessor, it will be called only if you don't set |retval|.
        /// Return true if interceptor retrieval was handled, false otherwise.
        /// </summary>
        // protected abstract int Get(cef_string_t* name, cef_v8value_t* @object, cef_v8value_t** retval, cef_string_t* exception);
        
        private int get_byindex(cef_v8interceptor_t* self, int index, cef_v8value_t* @object, cef_v8value_t** retval, cef_string_t* exception)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefV8Interceptor.Get
        }
        
        /// <summary>
        /// Handle retrieval of the interceptor value identified by |index|. |object|
        /// is the receiver ('this' object) of the interceptor. If retrieval succeeds,
        /// set |retval| to the return value. If the requested value does not exist,
        /// don't set either |retval| or |exception|. If retrieval fails, set
        /// |exception| to the exception that will be thrown.
        /// Return true if interceptor retrieval was handled, false otherwise.
        /// </summary>
        // protected abstract int Get(int index, cef_v8value_t* @object, cef_v8value_t** retval, cef_string_t* exception);
        
        private int set_byname(cef_v8interceptor_t* self, cef_string_t* name, cef_v8value_t* @object, cef_v8value_t* value, cef_string_t* exception)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefV8Interceptor.Set
        }
        
        /// <summary>
        /// Handle assignment of the interceptor value identified by |name|. |object|
        /// is the receiver ('this' object) of the interceptor. |value| is the new
        /// value being assigned to the interceptor. If assignment fails, set
        /// |exception| to the exception that will be thrown. This setter will always
        /// be called, even when the property has an associated accessor.
        /// Return true if interceptor assignment was handled, false otherwise.
        /// </summary>
        // protected abstract int Set(cef_string_t* name, cef_v8value_t* @object, cef_v8value_t* value, cef_string_t* exception);
        
        private int set_byindex(cef_v8interceptor_t* self, int index, cef_v8value_t* @object, cef_v8value_t* value, cef_string_t* exception)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefV8Interceptor.Set
        }
        
        /// <summary>
        /// Handle assignment of the interceptor value identified by |index|. |object|
        /// is the receiver ('this' object) of the interceptor. |value| is the new
        /// value being assigned to the interceptor. If assignment fails, set
        /// |exception| to the exception that will be thrown.
        /// Return true if interceptor assignment was handled, false otherwise.
        /// </summary>
        // protected abstract int Set(int index, cef_v8value_t* @object, cef_v8value_t* value, cef_string_t* exception);
        
    }
}
