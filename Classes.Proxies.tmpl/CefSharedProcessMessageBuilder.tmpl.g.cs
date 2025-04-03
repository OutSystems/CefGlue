namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that builds a CefProcessMessage containing a shared memory region.
    /// This class is not thread-safe but may be used exclusively on a different
    /// thread from the one which constructed it.
    /// </summary>
    public sealed unsafe partial class CefSharedProcessMessageBuilder
    {
        /// <summary>
        /// Creates a new CefSharedProcessMessageBuilder with the specified |name| and
        /// shared memory region of specified |byte_size|.
        /// </summary>
        public static cef_shared_process_message_builder_t* Create(cef_string_t* name, UIntPtr byte_size)
        {
            throw new NotImplementedException(); // TODO: CefSharedProcessMessageBuilder.Create
        }
        
        /// <summary>
        /// Returns true if the builder is valid.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefSharedProcessMessageBuilder.IsValid
        }
        
        /// <summary>
        /// Returns the size of the shared memory region in bytes. Returns 0 for
        /// invalid instances.
        /// </summary>
        public UIntPtr Size()
        {
            throw new NotImplementedException(); // TODO: CefSharedProcessMessageBuilder.Size
        }
        
        /// <summary>
        /// Returns the pointer to the writable memory. Returns nullptr for invalid
        /// instances. The returned pointer is only valid for the life span of this
        /// object.
        /// </summary>
        public void* Memory()
        {
            throw new NotImplementedException(); // TODO: CefSharedProcessMessageBuilder.Memory
        }
        
        /// <summary>
        /// Creates a new CefProcessMessage from the data provided to the builder.
        /// Returns nullptr for invalid instances. Invalidates the builder instance.
        /// </summary>
        public cef_process_message_t* Build()
        {
            throw new NotImplementedException(); // TODO: CefSharedProcessMessageBuilder.Build
        }
        
    }
}
