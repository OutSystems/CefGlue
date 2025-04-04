namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent a frame in the browser window. When used in the
    /// browser process the methods of this class may be called on any thread unless
    /// otherwise indicated in the comments. When used in the render process the
    /// methods of this class may only be called on the main thread.
    /// </summary>
    public sealed unsafe partial class CefFrame
    {
        /// <summary>
        /// True if this object is currently attached to a valid frame.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefFrame.IsValid
        }
        
        /// <summary>
        /// Execute undo in this frame.
        /// </summary>
        public void Undo()
        {
            throw new NotImplementedException(); // TODO: CefFrame.Undo
        }
        
        /// <summary>
        /// Execute redo in this frame.
        /// </summary>
        public void Redo()
        {
            throw new NotImplementedException(); // TODO: CefFrame.Redo
        }
        
        /// <summary>
        /// Execute cut in this frame.
        /// </summary>
        public void Cut()
        {
            throw new NotImplementedException(); // TODO: CefFrame.Cut
        }
        
        /// <summary>
        /// Execute copy in this frame.
        /// </summary>
        public void Copy()
        {
            throw new NotImplementedException(); // TODO: CefFrame.Copy
        }
        
        /// <summary>
        /// Execute paste in this frame.
        /// </summary>
        public void Paste()
        {
            throw new NotImplementedException(); // TODO: CefFrame.Paste
        }
        
        /// <summary>
        /// Execute paste and match style in this frame.
        /// </summary>
        public void PasteAndMatchStyle()
        {
            throw new NotImplementedException(); // TODO: CefFrame.PasteAndMatchStyle
        }
        
        /// <summary>
        /// Execute delete in this frame.
        /// </summary>
        public void Delete()
        {
            throw new NotImplementedException(); // TODO: CefFrame.Delete
        }
        
        /// <summary>
        /// Execute select all in this frame.
        /// </summary>
        public void SelectAll()
        {
            throw new NotImplementedException(); // TODO: CefFrame.SelectAll
        }
        
        /// <summary>
        /// Save this frame's HTML source to a temporary file and open it in the
        /// default text viewing application. This method can only be called from the
        /// browser process.
        /// </summary>
        public void ViewSource()
        {
            throw new NotImplementedException(); // TODO: CefFrame.ViewSource
        }
        
        /// <summary>
        /// Retrieve this frame's HTML source as a string sent to the specified
        /// visitor.
        /// </summary>
        public void GetSource(cef_string_visitor_t* visitor)
        {
            throw new NotImplementedException(); // TODO: CefFrame.GetSource
        }
        
        /// <summary>
        /// Retrieve this frame's display text as a string sent to the specified
        /// visitor.
        /// </summary>
        public void GetText(cef_string_visitor_t* visitor)
        {
            throw new NotImplementedException(); // TODO: CefFrame.GetText
        }
        
        /// <summary>
        /// Load the request represented by the |request| object.
        /// WARNING: This method will fail with "bad IPC message" reason
        /// INVALID_INITIATOR_ORIGIN (213) unless you first navigate to the
        /// request origin using some other mechanism (LoadURL, link click, etc).
        /// </summary>
        public void LoadRequest(cef_request_t* request)
        {
            throw new NotImplementedException(); // TODO: CefFrame.LoadRequest
        }
        
        /// <summary>
        /// Load the specified |url|.
        /// </summary>
        public void LoadURL(cef_string_t* url)
        {
            throw new NotImplementedException(); // TODO: CefFrame.LoadURL
        }
        
        /// <summary>
        /// Execute a string of JavaScript code in this frame. The |script_url|
        /// parameter is the URL where the script in question can be found, if any.
        /// The renderer may request this URL to show the developer the source of the
        /// error.  The |start_line| parameter is the base line number to use for
        /// error reporting.
        /// </summary>
        public void ExecuteJavaScript(cef_string_t* code, cef_string_t* script_url, int start_line)
        {
            throw new NotImplementedException(); // TODO: CefFrame.ExecuteJavaScript
        }
        
        /// <summary>
        /// Returns true if this is the main (top-level) frame.
        /// </summary>
        public int IsMain()
        {
            throw new NotImplementedException(); // TODO: CefFrame.IsMain
        }
        
        /// <summary>
        /// Returns true if this is the focused frame.
        /// </summary>
        public int IsFocused()
        {
            throw new NotImplementedException(); // TODO: CefFrame.IsFocused
        }
        
        /// <summary>
        /// Returns the name for this frame. If the frame has an assigned name (for
        /// example, set via the iframe "name" attribute) then that value will be
        /// returned. Otherwise a unique name will be constructed based on the frame
        /// parent hierarchy. The main (top-level) frame will always have an empty
        /// name value.
        /// </summary>
        public cef_string_userfree* GetName()
        {
            throw new NotImplementedException(); // TODO: CefFrame.GetName
        }
        
        /// <summary>
        /// Returns the globally unique identifier for this frame or empty if the
        /// underlying frame does not yet exist.
        /// </summary>
        public cef_string_userfree* GetIdentifier()
        {
            throw new NotImplementedException(); // TODO: CefFrame.GetIdentifier
        }
        
        /// <summary>
        /// Returns the parent of this frame or NULL if this is the main (top-level)
        /// frame.
        /// </summary>
        public cef_frame_t* GetParent()
        {
            throw new NotImplementedException(); // TODO: CefFrame.GetParent
        }
        
        /// <summary>
        /// Returns the URL currently loaded in this frame.
        /// </summary>
        public cef_string_userfree* GetURL()
        {
            throw new NotImplementedException(); // TODO: CefFrame.GetURL
        }
        
        /// <summary>
        /// Returns the browser that this frame belongs to.
        /// </summary>
        public cef_browser_t* GetBrowser()
        {
            throw new NotImplementedException(); // TODO: CefFrame.GetBrowser
        }
        
        /// <summary>
        /// Get the V8 context associated with the frame. This method can only be
        /// called from the render process.
        /// </summary>
        public cef_v8context_t* GetV8Context()
        {
            throw new NotImplementedException(); // TODO: CefFrame.GetV8Context
        }
        
        /// <summary>
        /// Visit the DOM document. This method can only be called from the render
        /// process.
        /// </summary>
        public void VisitDOM(cef_domvisitor_t* visitor)
        {
            throw new NotImplementedException(); // TODO: CefFrame.VisitDOM
        }
        
        /// <summary>
        /// Create a new URL request that will be treated as originating from this
        /// frame and the associated browser. Use CefURLRequest::Create instead if you
        /// do not want the request to have this association, in which case it may be
        /// handled differently (see documentation on that method). A request created
        /// with this method may only originate from the browser process, and will
        /// behave as follows:
        /// - It may be intercepted by the client via CefResourceRequestHandler or
        /// CefSchemeHandlerFactory.
        /// - POST data may only contain a single element of type PDE_TYPE_FILE or
        /// PDE_TYPE_BYTES.
        /// The |request| object will be marked as read-only after calling this
        /// method.
        /// </summary>
        public cef_urlrequest_t* CreateURLRequest(cef_request_t* request, cef_urlrequest_client_t* client)
        {
            throw new NotImplementedException(); // TODO: CefFrame.CreateURLRequest
        }
        
        /// <summary>
        /// Send a message to the specified |target_process|. Ownership of the message
        /// contents will be transferred and the |message| reference will be
        /// invalidated. Message delivery is not guaranteed in all cases (for example,
        /// if the browser is closing, navigating, or if the target process crashes).
        /// Send an ACK message back from the target process if confirmation is
        /// required.
        /// </summary>
        public void SendProcessMessage(CefProcessId target_process, cef_process_message_t* message)
        {
            throw new NotImplementedException(); // TODO: CefFrame.SendProcessMessage
        }
        
    }
}
