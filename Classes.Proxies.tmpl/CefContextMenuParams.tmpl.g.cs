namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Provides information about the context menu state. The methods of this class
    /// can only be accessed on browser process the UI thread.
    /// </summary>
    public sealed unsafe partial class CefContextMenuParams
    {
        /// <summary>
        /// Returns the X coordinate of the mouse where the context menu was invoked.
        /// Coords are relative to the associated RenderView's origin.
        /// </summary>
        public int GetXCoord()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetXCoord
        }
        
        /// <summary>
        /// Returns the Y coordinate of the mouse where the context menu was invoked.
        /// Coords are relative to the associated RenderView's origin.
        /// </summary>
        public int GetYCoord()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetYCoord
        }
        
        /// <summary>
        /// Returns flags representing the type of node that the context menu was
        /// invoked on.
        /// </summary>
        public CefContextMenuTypeFlags GetTypeFlags()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetTypeFlags
        }
        
        /// <summary>
        /// Returns the URL of the link, if any, that encloses the node that the
        /// context menu was invoked on.
        /// </summary>
        public cef_string_userfree* GetLinkUrl()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetLinkUrl
        }
        
        /// <summary>
        /// Returns the link URL, if any, to be used ONLY for "copy link address". We
        /// don't validate this field in the frontend process.
        /// </summary>
        public cef_string_userfree* GetUnfilteredLinkUrl()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetUnfilteredLinkUrl
        }
        
        /// <summary>
        /// Returns the source URL, if any, for the element that the context menu was
        /// invoked on. Example of elements with source URLs are img, audio, and
        /// video.
        /// </summary>
        public cef_string_userfree* GetSourceUrl()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetSourceUrl
        }
        
        /// <summary>
        /// Returns true if the context menu was invoked on an image which has
        /// non-empty contents.
        /// </summary>
        public int HasImageContents()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.HasImageContents
        }
        
        /// <summary>
        /// Returns the title text or the alt text if the context menu was invoked on
        /// an image.
        /// </summary>
        public cef_string_userfree* GetTitleText()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetTitleText
        }
        
        /// <summary>
        /// Returns the URL of the top level page that the context menu was invoked
        /// on.
        /// </summary>
        public cef_string_userfree* GetPageUrl()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetPageUrl
        }
        
        /// <summary>
        /// Returns the URL of the subframe that the context menu was invoked on.
        /// </summary>
        public cef_string_userfree* GetFrameUrl()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetFrameUrl
        }
        
        /// <summary>
        /// Returns the character encoding of the subframe that the context menu was
        /// invoked on.
        /// </summary>
        public cef_string_userfree* GetFrameCharset()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetFrameCharset
        }
        
        /// <summary>
        /// Returns the type of context node that the context menu was invoked on.
        /// </summary>
        public CefContextMenuMediaType GetMediaType()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetMediaType
        }
        
        /// <summary>
        /// Returns flags representing the actions supported by the media element, if
        /// any, that the context menu was invoked on.
        /// </summary>
        public CefContextMenuMediaStateFlags GetMediaStateFlags()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetMediaStateFlags
        }
        
        /// <summary>
        /// Returns the text of the selection, if any, that the context menu was
        /// invoked on.
        /// </summary>
        public cef_string_userfree* GetSelectionText()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetSelectionText
        }
        
        /// <summary>
        /// Returns the text of the misspelled word, if any, that the context menu was
        /// invoked on.
        /// </summary>
        public cef_string_userfree* GetMisspelledWord()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetMisspelledWord
        }
        
        /// <summary>
        /// Returns true if suggestions exist, false otherwise. Fills in |suggestions|
        /// from the spell check service for the misspelled word if there is one.
        /// </summary>
        public int GetDictionarySuggestions(cef_string_list* suggestions)
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetDictionarySuggestions
        }
        
        /// <summary>
        /// Returns true if the context menu was invoked on an editable node.
        /// </summary>
        public int IsEditable()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.IsEditable
        }
        
        /// <summary>
        /// Returns true if the context menu was invoked on an editable node where
        /// spell-check is enabled.
        /// </summary>
        public int IsSpellCheckEnabled()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.IsSpellCheckEnabled
        }
        
        /// <summary>
        /// Returns flags representing the actions supported by the editable node, if
        /// any, that the context menu was invoked on.
        /// </summary>
        public CefContextMenuEditStateFlags GetEditStateFlags()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.GetEditStateFlags
        }
        
        /// <summary>
        /// Returns true if the context menu contains items specified by the renderer
        /// process.
        /// </summary>
        public int IsCustomMenu()
        {
            throw new NotImplementedException(); // TODO: CefContextMenuParams.IsCustomMenu
        }
        
    }
}
