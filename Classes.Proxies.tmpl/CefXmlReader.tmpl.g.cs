namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that supports the reading of XML data via the libxml streaming API.
    /// The methods of this class should only be called on the thread that creates
    /// the object.
    /// </summary>
    public sealed unsafe partial class CefXmlReader
    {
        /// <summary>
        /// Create a new CefXmlReader object. The returned object's methods can only
        /// be called from the thread that created the object.
        /// </summary>
        public static cef_xml_reader_t* Create(cef_stream_reader_t* stream, CefXmlEncoding encodingType, cef_string_t* URI)
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.Create
        }
        
        /// <summary>
        /// Moves the cursor to the next node in the document. This method must be
        /// called at least once to set the current cursor position. Returns true if
        /// the cursor position was set successfully.
        /// </summary>
        public int MoveToNextNode()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.MoveToNextNode
        }
        
        /// <summary>
        /// Close the document. This should be called directly to ensure that cleanup
        /// occurs on the correct thread.
        /// </summary>
        public int Close()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.Close
        }
        
        /// <summary>
        /// Returns true if an error has been reported by the XML parser.
        /// </summary>
        public int HasError()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.HasError
        }
        
        /// <summary>
        /// Returns the error string.
        /// </summary>
        public cef_string_userfree* GetError()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetError
        }
        
        /// <summary>
        /// Returns the node type.
        /// </summary>
        public CefXmlNodeType GetType()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetType
        }
        
        /// <summary>
        /// Returns the node depth. Depth starts at 0 for the root node.
        /// </summary>
        public int GetDepth()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetDepth
        }
        
        /// <summary>
        /// Returns the local name. See
        /// http://www.w3.org/TR/REC-xml-names/#NT-LocalPart for additional details.
        /// </summary>
        public cef_string_userfree* GetLocalName()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetLocalName
        }
        
        /// <summary>
        /// Returns the namespace prefix. See http://www.w3.org/TR/REC-xml-names/ for
        /// additional details.
        /// </summary>
        public cef_string_userfree* GetPrefix()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetPrefix
        }
        
        /// <summary>
        /// Returns the qualified name, equal to (Prefix:)LocalName. See
        /// http://www.w3.org/TR/REC-xml-names/#ns-qualnames for additional details.
        /// </summary>
        public cef_string_userfree* GetQualifiedName()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetQualifiedName
        }
        
        /// <summary>
        /// Returns the URI defining the namespace associated with the node. See
        /// http://www.w3.org/TR/REC-xml-names/ for additional details.
        /// </summary>
        public cef_string_userfree* GetNamespaceURI()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetNamespaceURI
        }
        
        /// <summary>
        /// Returns the base URI of the node. See http://www.w3.org/TR/xmlbase/ for
        /// additional details.
        /// </summary>
        public cef_string_userfree* GetBaseURI()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetBaseURI
        }
        
        /// <summary>
        /// Returns the xml:lang scope within which the node resides. See
        /// http://www.w3.org/TR/REC-xml/#sec-lang-tag for additional details.
        /// </summary>
        public cef_string_userfree* GetXmlLang()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetXmlLang
        }
        
        /// <summary>
        /// Returns true if the node represents an empty element. "&lt;a/&gt;" is considered
        /// empty but "&lt;a&gt;&lt;/a&gt;" is not.
        /// </summary>
        public int IsEmptyElement()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.IsEmptyElement
        }
        
        /// <summary>
        /// Returns true if the node has a text value.
        /// </summary>
        public int HasValue()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.HasValue
        }
        
        /// <summary>
        /// Returns the text value.
        /// </summary>
        public cef_string_userfree* GetValue()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetValue
        }
        
        /// <summary>
        /// Returns true if the node has attributes.
        /// </summary>
        public int HasAttributes()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.HasAttributes
        }
        
        /// <summary>
        /// Returns the number of attributes.
        /// </summary>
        public UIntPtr GetAttributeCount()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetAttributeCount
        }
        
        /// <summary>
        /// Returns the value of the attribute at the specified 0-based index.
        /// </summary>
        public cef_string_userfree* GetAttribute(int index)
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetAttribute
        }
        
        /// <summary>
        /// Returns the value of the attribute with the specified qualified name.
        /// </summary>
        public cef_string_userfree* GetAttribute(cef_string_t* qualifiedName)
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetAttribute
        }
        
        /// <summary>
        /// Returns the value of the attribute with the specified local name and
        /// namespace URI.
        /// </summary>
        public cef_string_userfree* GetAttribute(cef_string_t* localName, cef_string_t* namespaceURI)
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetAttribute
        }
        
        /// <summary>
        /// Returns an XML representation of the current node's children.
        /// </summary>
        public cef_string_userfree* GetInnerXml()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetInnerXml
        }
        
        /// <summary>
        /// Returns an XML representation of the current node including its children.
        /// </summary>
        public cef_string_userfree* GetOuterXml()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetOuterXml
        }
        
        /// <summary>
        /// Returns the line number for the current node.
        /// </summary>
        public int GetLineNumber()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.GetLineNumber
        }
        
        /// <summary>
        /// Moves the cursor to the attribute at the specified 0-based index. Returns
        /// true if the cursor position was set successfully.
        /// </summary>
        public int MoveToAttribute(int index)
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.MoveToAttribute
        }
        
        /// <summary>
        /// Moves the cursor to the attribute with the specified qualified name.
        /// Returns true if the cursor position was set successfully.
        /// </summary>
        public int MoveToAttribute(cef_string_t* qualifiedName)
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.MoveToAttribute
        }
        
        /// <summary>
        /// Moves the cursor to the attribute with the specified local name and
        /// namespace URI. Returns true if the cursor position was set successfully.
        /// </summary>
        public int MoveToAttribute(cef_string_t* localName, cef_string_t* namespaceURI)
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.MoveToAttribute
        }
        
        /// <summary>
        /// Moves the cursor to the first attribute in the current element. Returns
        /// true if the cursor position was set successfully.
        /// </summary>
        public int MoveToFirstAttribute()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.MoveToFirstAttribute
        }
        
        /// <summary>
        /// Moves the cursor to the next attribute in the current element. Returns
        /// true if the cursor position was set successfully.
        /// </summary>
        public int MoveToNextAttribute()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.MoveToNextAttribute
        }
        
        /// <summary>
        /// Moves the cursor back to the carrying element. Returns true if the cursor
        /// position was set successfully.
        /// </summary>
        public int MoveToCarryingElement()
        {
            throw new NotImplementedException(); // TODO: CefXmlReader.MoveToCarryingElement
        }
        
    }
}
