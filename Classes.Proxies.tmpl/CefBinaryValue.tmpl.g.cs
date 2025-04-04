namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a binary value. Can be used on any process and thread.
    /// </summary>
    public sealed unsafe partial class CefBinaryValue
    {
        /// <summary>
        /// Creates a new object that is not owned by any other object. The specified
        /// |data| will be copied.
        /// </summary>
        public static cef_binary_value_t* Create(void* data, UIntPtr data_size)
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.Create
        }
        
        /// <summary>
        /// Returns true if this object is valid. This object may become invalid if
        /// the underlying data is owned by another object (e.g. list or dictionary)
        /// and that other object is then modified or destroyed. Do not call any other
        /// methods if this method returns false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.IsValid
        }
        
        /// <summary>
        /// Returns true if this object is currently owned by another object.
        /// </summary>
        public int IsOwned()
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.IsOwned
        }
        
        /// <summary>
        /// Returns true if this object and |that| object have the same underlying
        /// data.
        /// </summary>
        public int IsSame(cef_binary_value_t* that)
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.IsSame
        }
        
        /// <summary>
        /// Returns true if this object and |that| object have an equivalent
        /// underlying value but are not necessarily the same object.
        /// </summary>
        public int IsEqual(cef_binary_value_t* that)
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.IsEqual
        }
        
        /// <summary>
        /// Returns a copy of this object. The data in this object will also be
        /// copied.
        /// </summary>
        public cef_binary_value_t* Copy()
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.Copy
        }
        
        /// <summary>
        /// Returns a pointer to the beginning of the memory block.
        /// The returned pointer is valid as long as the CefBinaryValue is alive.
        /// </summary>
        public void* GetRawData()
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.GetRawData
        }
        
        /// <summary>
        /// Returns the data size.
        /// </summary>
        public UIntPtr GetSize()
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.GetSize
        }
        
        /// <summary>
        /// Read up to |buffer_size| number of bytes into |buffer|. Reading begins at
        /// the specified byte |data_offset|. Returns the number of bytes read.
        /// </summary>
        public UIntPtr GetData(void* buffer, UIntPtr buffer_size, UIntPtr data_offset)
        {
            throw new NotImplementedException(); // TODO: CefBinaryValue.GetData
        }
        
    }
}
