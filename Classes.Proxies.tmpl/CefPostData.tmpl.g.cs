namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent post data for a web request. The methods of this
    /// class may be called on any thread.
    /// </summary>
    public sealed unsafe partial class CefPostData
    {
        /// <summary>
        /// Create a new CefPostData object.
        /// </summary>
        public static cef_post_data_t* Create()
        {
            throw new NotImplementedException(); // TODO: CefPostData.Create
        }
        
        /// <summary>
        /// Returns true if this object is read-only.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefPostData.IsReadOnly
        }
        
        /// <summary>
        /// Returns true if the underlying POST data includes elements that are not
        /// represented by this CefPostData object (for example, multi-part file
        /// upload data). Modifying CefPostData objects with excluded elements may
        /// result in the request failing.
        /// </summary>
        public int HasExcludedElements()
        {
            throw new NotImplementedException(); // TODO: CefPostData.HasExcludedElements
        }
        
        /// <summary>
        /// Returns the number of existing post data elements.
        /// </summary>
        public UIntPtr GetElementCount()
        {
            throw new NotImplementedException(); // TODO: CefPostData.GetElementCount
        }
        
        /// <summary>
        /// Retrieve the post data elements.
        /// </summary>
        public void GetElements(UIntPtr* elementsCount, cef_post_data_element_t** elements)
        {
            throw new NotImplementedException(); // TODO: CefPostData.GetElements
        }
        
        /// <summary>
        /// Remove the specified post data element.  Returns true if the removal
        /// succeeds.
        /// </summary>
        public int RemoveElement(cef_post_data_element_t* element)
        {
            throw new NotImplementedException(); // TODO: CefPostData.RemoveElement
        }
        
        /// <summary>
        /// Add the specified post data element.  Returns true if the add succeeds.
        /// </summary>
        public int AddElement(cef_post_data_element_t* element)
        {
            throw new NotImplementedException(); // TODO: CefPostData.AddElement
        }
        
        /// <summary>
        /// Remove all existing post data elements.
        /// </summary>
        public void RemoveElements()
        {
            throw new NotImplementedException(); // TODO: CefPostData.RemoveElements
        }
        
    }
}
