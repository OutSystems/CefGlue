namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that wraps platform-dependent share memory region mapping.
    /// </summary>
    public sealed unsafe partial class CefSharedMemoryRegion
    {
        /// <summary>
        /// Returns true if the mapping is valid.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefSharedMemoryRegion.IsValid
        }
        
        /// <summary>
        /// Returns the size of the mapping in bytes. Returns 0 for invalid instances.
        /// </summary>
        public UIntPtr Size()
        {
            throw new NotImplementedException(); // TODO: CefSharedMemoryRegion.Size
        }
        
        /// <summary>
        /// Returns the pointer to the memory. Returns nullptr for invalid instances.
        /// The returned pointer is only valid for the life span of this object.
        /// </summary>
        public void* Memory()
        {
            throw new NotImplementedException(); // TODO: CefSharedMemoryRegion.Memory
        }
        
    }
}
