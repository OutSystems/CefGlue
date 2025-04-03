namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Interface that should be implemented to handle V8 accessor calls. Accessor
    /// identifiers are registered by calling CefV8Value::SetValue(). The methods
    /// of this class will be called on the thread associated with the V8 accessor.
    /// </summary>
    public abstract unsafe partial class CefV8Accessor
    {
        private int get(cef_v8accessor_t* self, cef_string_t* name, cef_v8value_t* @object, cef_v8value_t** retval, cef_string_t* exception)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefV8Accessor.Get
        }
        
        /// <summary>
        /// Handle retrieval the accessor value identified by |name|. |object| is the
        /// receiver ('this' object) of the accessor. If retrieval succeeds set
        /// |retval| to the return value. If retrieval fails set |exception| to the
        /// exception that will be thrown. Return true if accessor retrieval was
        /// handled.
        /// </summary>
        // protected abstract int Get(cef_string_t* name, cef_v8value_t* @object, cef_v8value_t** retval, cef_string_t* exception);
        
        private int set(cef_v8accessor_t* self, cef_string_t* name, cef_v8value_t* @object, cef_v8value_t* value, cef_string_t* exception)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefV8Accessor.Set
        }
        
        /// <summary>
        /// Handle assignment of the accessor value identified by |name|. |object| is
        /// the receiver ('this' object) of the accessor. |value| is the new value
        /// being assigned to the accessor. If assignment fails set |exception| to the
        /// exception that will be thrown. Return true if accessor assignment was
        /// handled.
        /// </summary>
        // protected abstract int Set(cef_string_t* name, cef_v8value_t* @object, cef_v8value_t* value, cef_string_t* exception);
        
    }
}
