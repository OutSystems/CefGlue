namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent a single element in the request post data. The
    /// methods of this class may be called on any thread.
    /// </summary>
    public sealed unsafe partial class CefPostDataElement
    {
        /// <summary>
        /// Create a new CefPostDataElement object.
        /// </summary>
        public static cef_post_data_element_t* Create()
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.Create
        }
        
        /// <summary>
        /// Returns true if this object is read-only.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.IsReadOnly
        }
        
        /// <summary>
        /// Remove all contents from the post data element.
        /// </summary>
        public void SetToEmpty()
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.SetToEmpty
        }
        
        /// <summary>
        /// The post data element will represent a file.
        /// </summary>
        public void SetToFile(cef_string_t* fileName)
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.SetToFile
        }
        
        /// <summary>
        /// The post data element will represent bytes.  The bytes passed
        /// in will be copied.
        /// </summary>
        public void SetToBytes(UIntPtr size, void* bytes)
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.SetToBytes
        }
        
        /// <summary>
        /// Return the type of this post data element.
        /// </summary>
        public CefPostDataElementType GetType()
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.GetType
        }
        
        /// <summary>
        /// Return the file name.
        /// </summary>
        public cef_string_userfree* GetFile()
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.GetFile
        }
        
        /// <summary>
        /// Return the number of bytes.
        /// </summary>
        public UIntPtr GetBytesCount()
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.GetBytesCount
        }
        
        /// <summary>
        /// Read up to |size| bytes into |bytes| and return the number of bytes
        /// actually read.
        /// </summary>
        public UIntPtr GetBytes(UIntPtr size, void* bytes)
        {
            throw new NotImplementedException(); // TODO: CefPostDataElement.GetBytes
        }
        
    }
}
