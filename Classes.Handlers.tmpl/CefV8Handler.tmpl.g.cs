namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Interface that should be implemented to handle V8 function calls. The
    /// methods of this class will be called on the thread associated with the V8
    /// function.
    /// </summary>
    public abstract unsafe partial class CefV8Handler
    {
        private int execute(cef_v8handler_t* self, cef_string_t* name, cef_v8value_t* @object, UIntPtr argumentsCount, cef_v8value_t** arguments, cef_v8value_t** retval, cef_string_t* exception)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefV8Handler.Execute
        }
        
        /// <summary>
        /// Handle execution of the function identified by |name|. |object| is the
        /// receiver ('this' object) of the function. |arguments| is the list of
        /// arguments passed to the function. If execution succeeds set |retval| to
        /// the function return value. If execution fails set |exception| to the
        /// exception that will be thrown. Return true if execution was handled.
        /// </summary>
        // protected abstract int Execute(cef_string_t* name, cef_v8value_t* @object, UIntPtr argumentsCount, cef_v8value_t** arguments, cef_v8value_t** retval, cef_string_t* exception);
        
    }
}
