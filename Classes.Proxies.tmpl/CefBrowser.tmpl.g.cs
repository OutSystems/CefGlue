namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent a browser. When used in the browser process the
    /// methods of this class may be called on any thread unless otherwise indicated
    /// in the comments. When used in the render process the methods of this class
    /// may only be called on the main thread.
    /// </summary>
    public sealed unsafe partial class CefBrowser
    {
        /// <summary>
        /// True if this object is currently valid. This will return false after
        /// CefLifeSpanHandler::OnBeforeClose is called.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.IsValid
        }
        
        /// <summary>
        /// Returns the browser host object. This method can only be called in the
        /// browser process.
        /// </summary>
        public cef_browser_host_t* GetHost()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetHost
        }
        
        /// <summary>
        /// Returns true if the browser can navigate backwards.
        /// </summary>
        public int CanGoBack()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.CanGoBack
        }
        
        /// <summary>
        /// Navigate backwards.
        /// </summary>
        public void GoBack()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GoBack
        }
        
        /// <summary>
        /// Returns true if the browser can navigate forwards.
        /// </summary>
        public int CanGoForward()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.CanGoForward
        }
        
        /// <summary>
        /// Navigate forwards.
        /// </summary>
        public void GoForward()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GoForward
        }
        
        /// <summary>
        /// Returns true if the browser is currently loading.
        /// </summary>
        public int IsLoading()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.IsLoading
        }
        
        /// <summary>
        /// Reload the current page.
        /// </summary>
        public void Reload()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.Reload
        }
        
        /// <summary>
        /// Reload the current page ignoring any cached data.
        /// </summary>
        public void ReloadIgnoreCache()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.ReloadIgnoreCache
        }
        
        /// <summary>
        /// Stop loading the page.
        /// </summary>
        public void StopLoad()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.StopLoad
        }
        
        /// <summary>
        /// Returns the globally unique identifier for this browser. This value is
        /// also used as the tabId for extension APIs.
        /// </summary>
        public int GetIdentifier()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetIdentifier
        }
        
        /// <summary>
        /// Returns true if this object is pointing to the same handle as |that|
        /// object.
        /// </summary>
        public int IsSame(cef_browser_t* that)
        {
            throw new NotImplementedException(); // TODO: CefBrowser.IsSame
        }
        
        /// <summary>
        /// Returns true if the browser is a popup.
        /// </summary>
        public int IsPopup()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.IsPopup
        }
        
        /// <summary>
        /// Returns true if a document has been loaded in the browser.
        /// </summary>
        public int HasDocument()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.HasDocument
        }
        
        /// <summary>
        /// Returns the main (top-level) frame for the browser. In the browser process
        /// this will return a valid object until after
        /// CefLifeSpanHandler::OnBeforeClose is called. In the renderer process this
        /// will return NULL if the main frame is hosted in a different renderer
        /// process (e.g. for cross-origin sub-frames). The main frame object will
        /// change during cross-origin navigation or re-navigation after renderer
        /// process termination (due to crashes, etc).
        /// </summary>
        public cef_frame_t* GetMainFrame()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetMainFrame
        }
        
        /// <summary>
        /// Returns the focused frame for the browser.
        /// </summary>
        public cef_frame_t* GetFocusedFrame()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetFocusedFrame
        }
        
        /// <summary>
        /// Returns the frame with the specified identifier, or NULL if not found.
        /// </summary>
        public cef_frame_t* GetFrameByIdentifier(cef_string_t* identifier)
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetFrameByIdentifier
        }
        
        /// <summary>
        /// Returns the frame with the specified name, or NULL if not found.
        /// </summary>
        public cef_frame_t* GetFrameByName(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetFrameByName
        }
        
        /// <summary>
        /// Returns the number of frames that currently exist.
        /// </summary>
        public UIntPtr GetFrameCount()
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetFrameCount
        }
        
        /// <summary>
        /// Returns the identifiers of all existing frames.
        /// </summary>
        public void GetFrameIdentifiers(cef_string_list* identifiers)
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetFrameIdentifiers
        }
        
        /// <summary>
        /// Returns the names of all existing frames.
        /// </summary>
        public void GetFrameNames(cef_string_list* names)
        {
            throw new NotImplementedException(); // TODO: CefBrowser.GetFrameNames
        }
        
    }
}
