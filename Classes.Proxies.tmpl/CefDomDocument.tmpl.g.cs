namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent a DOM document. The methods of this class should
    /// only be called on the render process main thread thread.
    /// </summary>
    public sealed unsafe partial class CefDomDocument
    {
        /// <summary>
        /// Returns the document type.
        /// </summary>
        public CefDomDocumentType GetType()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetType
        }
        
        /// <summary>
        /// Returns the root document node.
        /// </summary>
        public cef_domnode_t* GetDocument()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetDocument
        }
        
        /// <summary>
        /// Returns the BODY node of an HTML document.
        /// </summary>
        public cef_domnode_t* GetBody()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetBody
        }
        
        /// <summary>
        /// Returns the HEAD node of an HTML document.
        /// </summary>
        public cef_domnode_t* GetHead()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetHead
        }
        
        /// <summary>
        /// Returns the title of an HTML document.
        /// </summary>
        public cef_string_userfree* GetTitle()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetTitle
        }
        
        /// <summary>
        /// Returns the document element with the specified ID value.
        /// </summary>
        public cef_domnode_t* GetElementById(cef_string_t* id)
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetElementById
        }
        
        /// <summary>
        /// Returns the node that currently has keyboard focus.
        /// </summary>
        public cef_domnode_t* GetFocusedNode()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetFocusedNode
        }
        
        /// <summary>
        /// Returns true if a portion of the document is selected.
        /// </summary>
        public int HasSelection()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.HasSelection
        }
        
        /// <summary>
        /// Returns the selection offset within the start node.
        /// </summary>
        public int GetSelectionStartOffset()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetSelectionStartOffset
        }
        
        /// <summary>
        /// Returns the selection offset within the end node.
        /// </summary>
        public int GetSelectionEndOffset()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetSelectionEndOffset
        }
        
        /// <summary>
        /// Returns the contents of this selection as markup.
        /// </summary>
        public cef_string_userfree* GetSelectionAsMarkup()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetSelectionAsMarkup
        }
        
        /// <summary>
        /// Returns the contents of this selection as text.
        /// </summary>
        public cef_string_userfree* GetSelectionAsText()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetSelectionAsText
        }
        
        /// <summary>
        /// Returns the base URL for the document.
        /// </summary>
        public cef_string_userfree* GetBaseURL()
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetBaseURL
        }
        
        /// <summary>
        /// Returns a complete URL based on the document base URL and the specified
        /// partial URL.
        /// </summary>
        public cef_string_userfree* GetCompleteURL(cef_string_t* partialURL)
        {
            throw new NotImplementedException(); // TODO: CefDomDocument.GetCompleteURL
        }
        
    }
}
