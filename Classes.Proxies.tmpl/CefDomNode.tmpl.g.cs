namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent a DOM node. The methods of this class should only be
    /// called on the render process main thread.
    /// </summary>
    public sealed unsafe partial class CefDomNode
    {
        /// <summary>
        /// Returns the type for this node.
        /// </summary>
        public CefDomNodeType GetType()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetType
        }
        
        /// <summary>
        /// Returns true if this is a text node.
        /// </summary>
        public int IsText()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.IsText
        }
        
        /// <summary>
        /// Returns true if this is an element node.
        /// </summary>
        public int IsElement()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.IsElement
        }
        
        /// <summary>
        /// Returns true if this is an editable node.
        /// </summary>
        public int IsEditable()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.IsEditable
        }
        
        /// <summary>
        /// Returns true if this is a form control element node.
        /// </summary>
        public int IsFormControlElement()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.IsFormControlElement
        }
        
        /// <summary>
        /// Returns the type of this form control element node.
        /// </summary>
        public CefDomFormControlType GetFormControlElementType()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetFormControlElementType
        }
        
        /// <summary>
        /// Returns true if this object is pointing to the same handle as |that|
        /// object.
        /// </summary>
        public int IsSame(cef_domnode_t* that)
        {
            throw new NotImplementedException(); // TODO: CefDomNode.IsSame
        }
        
        /// <summary>
        /// Returns the name of this node.
        /// </summary>
        public cef_string_userfree* GetName()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetName
        }
        
        /// <summary>
        /// Returns the value of this node.
        /// </summary>
        public cef_string_userfree* GetValue()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetValue
        }
        
        /// <summary>
        /// Set the value of this node. Returns true on success.
        /// </summary>
        public int SetValue(cef_string_t* value)
        {
            throw new NotImplementedException(); // TODO: CefDomNode.SetValue
        }
        
        /// <summary>
        /// Returns the contents of this node as markup.
        /// </summary>
        public cef_string_userfree* GetAsMarkup()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetAsMarkup
        }
        
        /// <summary>
        /// Returns the document associated with this node.
        /// </summary>
        public cef_domdocument_t* GetDocument()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetDocument
        }
        
        /// <summary>
        /// Returns the parent node.
        /// </summary>
        public cef_domnode_t* GetParent()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetParent
        }
        
        /// <summary>
        /// Returns the previous sibling node.
        /// </summary>
        public cef_domnode_t* GetPreviousSibling()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetPreviousSibling
        }
        
        /// <summary>
        /// Returns the next sibling node.
        /// </summary>
        public cef_domnode_t* GetNextSibling()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetNextSibling
        }
        
        /// <summary>
        /// Returns true if this node has child nodes.
        /// </summary>
        public int HasChildren()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.HasChildren
        }
        
        /// <summary>
        /// Return the first child node.
        /// </summary>
        public cef_domnode_t* GetFirstChild()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetFirstChild
        }
        
        /// <summary>
        /// Returns the last child node.
        /// </summary>
        public cef_domnode_t* GetLastChild()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetLastChild
        }
        
        /// <summary>
        /// Returns the tag name of this element.
        /// </summary>
        public cef_string_userfree* GetElementTagName()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetElementTagName
        }
        
        /// <summary>
        /// Returns true if this element has attributes.
        /// </summary>
        public int HasElementAttributes()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.HasElementAttributes
        }
        
        /// <summary>
        /// Returns true if this element has an attribute named |attrName|.
        /// </summary>
        public int HasElementAttribute(cef_string_t* attrName)
        {
            throw new NotImplementedException(); // TODO: CefDomNode.HasElementAttribute
        }
        
        /// <summary>
        /// Returns the element attribute named |attrName|.
        /// </summary>
        public cef_string_userfree* GetElementAttribute(cef_string_t* attrName)
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetElementAttribute
        }
        
        /// <summary>
        /// Returns a map of all element attributes.
        /// </summary>
        public void GetElementAttributes(cef_string_map* attrMap)
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetElementAttributes
        }
        
        /// <summary>
        /// Set the value for the element attribute named |attrName|. Returns true on
        /// success.
        /// </summary>
        public int SetElementAttribute(cef_string_t* attrName, cef_string_t* value)
        {
            throw new NotImplementedException(); // TODO: CefDomNode.SetElementAttribute
        }
        
        /// <summary>
        /// Returns the inner text of the element.
        /// </summary>
        public cef_string_userfree* GetElementInnerText()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetElementInnerText
        }
        
        /// <summary>
        /// Returns the bounds of the element in device pixels. Use
        /// "window.devicePixelRatio" to convert to/from CSS pixels.
        /// </summary>
        public cef_rect_t GetElementBounds()
        {
            throw new NotImplementedException(); // TODO: CefDomNode.GetElementBounds
        }
        
    }
}
