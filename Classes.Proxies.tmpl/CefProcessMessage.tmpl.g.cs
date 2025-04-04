namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a message. Can be used on any process and thread.
    /// </summary>
    public sealed unsafe partial class CefProcessMessage
    {
        /// <summary>
        /// Create a new CefProcessMessage object with the specified name.
        /// </summary>
        public static cef_process_message_t* Create(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefProcessMessage.Create
        }
        
        /// <summary>
        /// Returns true if this object is valid. Do not call any other methods if
        /// this function returns false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefProcessMessage.IsValid
        }
        
        /// <summary>
        /// Returns true if the values of this object are read-only. Some APIs may
        /// expose read-only objects.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefProcessMessage.IsReadOnly
        }
        
        /// <summary>
        /// Returns a writable copy of this object.
        /// Returns nullptr when message contains a shared memory region.
        /// </summary>
        public cef_process_message_t* Copy()
        {
            throw new NotImplementedException(); // TODO: CefProcessMessage.Copy
        }
        
        /// <summary>
        /// Returns the message name.
        /// </summary>
        public cef_string_userfree* GetName()
        {
            throw new NotImplementedException(); // TODO: CefProcessMessage.GetName
        }
        
        /// <summary>
        /// Returns the list of arguments.
        /// Returns nullptr when message contains a shared memory region.
        /// </summary>
        public cef_list_value_t* GetArgumentList()
        {
            throw new NotImplementedException(); // TODO: CefProcessMessage.GetArgumentList
        }
        
        /// <summary>
        /// Returns the shared memory region.
        /// Returns nullptr when message contains an argument list.
        /// </summary>
        public cef_shared_memory_region_t* GetSharedMemoryRegion()
        {
            throw new NotImplementedException(); // TODO: CefProcessMessage.GetSharedMemoryRegion
        }
        
    }
}
