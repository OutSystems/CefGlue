namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent drag data. The methods of this class may be called
    /// on any thread.
    /// </summary>
    public sealed unsafe partial class CefDragData
    {
        /// <summary>
        /// Create a new CefDragData object.
        /// </summary>
        public static cef_drag_data_t* Create()
        {
            throw new NotImplementedException(); // TODO: CefDragData.Create
        }
        
        /// <summary>
        /// Returns a copy of the current object.
        /// </summary>
        public cef_drag_data_t* Clone()
        {
            throw new NotImplementedException(); // TODO: CefDragData.Clone
        }
        
        /// <summary>
        /// Returns true if this object is read-only.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefDragData.IsReadOnly
        }
        
        /// <summary>
        /// Returns true if the drag data is a link.
        /// </summary>
        public int IsLink()
        {
            throw new NotImplementedException(); // TODO: CefDragData.IsLink
        }
        
        /// <summary>
        /// Returns true if the drag data is a text or html fragment.
        /// </summary>
        public int IsFragment()
        {
            throw new NotImplementedException(); // TODO: CefDragData.IsFragment
        }
        
        /// <summary>
        /// Returns true if the drag data is a file.
        /// </summary>
        public int IsFile()
        {
            throw new NotImplementedException(); // TODO: CefDragData.IsFile
        }
        
        /// <summary>
        /// Return the link URL that is being dragged.
        /// </summary>
        public cef_string_userfree* GetLinkURL()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetLinkURL
        }
        
        /// <summary>
        /// Return the title associated with the link being dragged.
        /// </summary>
        public cef_string_userfree* GetLinkTitle()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetLinkTitle
        }
        
        /// <summary>
        /// Return the metadata, if any, associated with the link being dragged.
        /// </summary>
        public cef_string_userfree* GetLinkMetadata()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetLinkMetadata
        }
        
        /// <summary>
        /// Return the plain text fragment that is being dragged.
        /// </summary>
        public cef_string_userfree* GetFragmentText()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetFragmentText
        }
        
        /// <summary>
        /// Return the text/html fragment that is being dragged.
        /// </summary>
        public cef_string_userfree* GetFragmentHtml()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetFragmentHtml
        }
        
        /// <summary>
        /// Return the base URL that the fragment came from. This value is used for
        /// resolving relative URLs and may be empty.
        /// </summary>
        public cef_string_userfree* GetFragmentBaseURL()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetFragmentBaseURL
        }
        
        /// <summary>
        /// Return the name of the file being dragged out of the browser window.
        /// </summary>
        public cef_string_userfree* GetFileName()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetFileName
        }
        
        /// <summary>
        /// Write the contents of the file being dragged out of the web view into
        /// |writer|. Returns the number of bytes sent to |writer|. If |writer| is
        /// NULL this method will return the size of the file contents in bytes.
        /// Call GetFileName() to get a suggested name for the file.
        /// </summary>
        public UIntPtr GetFileContents(cef_stream_writer_t* writer)
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetFileContents
        }
        
        /// <summary>
        /// Retrieve the list of file names that are being dragged into the browser
        /// window.
        /// </summary>
        public int GetFileNames(cef_string_list* names)
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetFileNames
        }
        
        /// <summary>
        /// Retrieve the list of file paths that are being dragged into the browser
        /// window.
        /// </summary>
        public int GetFilePaths(cef_string_list* paths)
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetFilePaths
        }
        
        /// <summary>
        /// Set the link URL that is being dragged.
        /// </summary>
        public void SetLinkURL(cef_string_t* url)
        {
            throw new NotImplementedException(); // TODO: CefDragData.SetLinkURL
        }
        
        /// <summary>
        /// Set the title associated with the link being dragged.
        /// </summary>
        public void SetLinkTitle(cef_string_t* title)
        {
            throw new NotImplementedException(); // TODO: CefDragData.SetLinkTitle
        }
        
        /// <summary>
        /// Set the metadata associated with the link being dragged.
        /// </summary>
        public void SetLinkMetadata(cef_string_t* data)
        {
            throw new NotImplementedException(); // TODO: CefDragData.SetLinkMetadata
        }
        
        /// <summary>
        /// Set the plain text fragment that is being dragged.
        /// </summary>
        public void SetFragmentText(cef_string_t* text)
        {
            throw new NotImplementedException(); // TODO: CefDragData.SetFragmentText
        }
        
        /// <summary>
        /// Set the text/html fragment that is being dragged.
        /// </summary>
        public void SetFragmentHtml(cef_string_t* html)
        {
            throw new NotImplementedException(); // TODO: CefDragData.SetFragmentHtml
        }
        
        /// <summary>
        /// Set the base URL that the fragment came from.
        /// </summary>
        public void SetFragmentBaseURL(cef_string_t* base_url)
        {
            throw new NotImplementedException(); // TODO: CefDragData.SetFragmentBaseURL
        }
        
        /// <summary>
        /// Reset the file contents. You should do this before calling
        /// CefBrowserHost::DragTargetDragEnter as the web view does not allow us to
        /// drag in this kind of data.
        /// </summary>
        public void ResetFileContents()
        {
            throw new NotImplementedException(); // TODO: CefDragData.ResetFileContents
        }
        
        /// <summary>
        /// Add a file that is being dragged into the webview.
        /// </summary>
        public void AddFile(cef_string_t* path, cef_string_t* display_name)
        {
            throw new NotImplementedException(); // TODO: CefDragData.AddFile
        }
        
        /// <summary>
        /// Clear list of filenames.
        /// </summary>
        public void ClearFilenames()
        {
            throw new NotImplementedException(); // TODO: CefDragData.ClearFilenames
        }
        
        /// <summary>
        /// Get the image representation of drag data. May return NULL if no image
        /// representation is available.
        /// </summary>
        public cef_image_t* GetImage()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetImage
        }
        
        /// <summary>
        /// Get the image hotspot (drag start location relative to image dimensions).
        /// </summary>
        public cef_point_t GetImageHotspot()
        {
            throw new NotImplementedException(); // TODO: CefDragData.GetImageHotspot
        }
        
        /// <summary>
        /// Returns true if an image representation of drag data is available.
        /// </summary>
        public int HasImage()
        {
            throw new NotImplementedException(); // TODO: CefDragData.HasImage
        }
        
    }
}
