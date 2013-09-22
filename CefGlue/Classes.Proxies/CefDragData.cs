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
        /// Returns true if the drag data is a link.
        /// </summary>
        public bool IsLink
        {
            get { return cef_drag_data_t.is_link(_self) != 0; }
        }

        /// <summary>
        /// Returns true if the drag data is a text or html fragment.
        /// </summary>
        public bool IsFragment
        {
            get { return cef_drag_data_t.is_fragment(_self) != 0; }
        }

        /// <summary>
        /// Returns true if the drag data is a file.
        /// </summary>
        public bool IsFile
        {
            get { return cef_drag_data_t.is_file(_self) != 0; }
        }

        /// <summary>
        /// Return the link URL that is being dragged.
        /// </summary>
        public string LinkUrl
        {
            get
            {
                var n_result = cef_drag_data_t.get_link_url(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Return the title associated with the link being dragged.
        /// </summary>
        public string LinkTitle
        {
            get
            {
                var n_result = cef_drag_data_t.get_link_title(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Return the metadata, if any, associated with the link being dragged.
        /// </summary>
        public string LinkMetadata
        {
            get
            {
                var n_result = cef_drag_data_t.get_link_metadata(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Return the plain text fragment that is being dragged.
        /// </summary>
        public string FragmentText
        {
            get
            {
                var n_result = cef_drag_data_t.get_fragment_text(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Return the text/html fragment that is being dragged.
        /// </summary>
        public string FragmentHtml
        {
            get
            {
                var n_result = cef_drag_data_t.get_fragment_html(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Return the base URL that the fragment came from. This value is used for
        /// resolving relative URLs and may be empty.
        /// </summary>
        public string FragmentBaseUrl
        {
            get
            {
                var n_result = cef_drag_data_t.get_fragment_base_url(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Return the name of the file being dragged out of the browser window.
        /// </summary>
        public string FileName
        {
            get
            {
                var n_result = cef_drag_data_t.get_file_name(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Retrieve the list of file names that are being dragged into the browser
        /// window.
        /// </summary>
        public string[] GetFileNames()
        {
            cef_string_list* n_result = null;
            try
            {
                n_result = libcef.string_list_alloc();
                var success = cef_drag_data_t.get_file_names(_self, n_result) != 0;
                if (!success) return null;
                return cef_string_list.ToArray(n_result);
            }
            finally
            {
                if (n_result != null) libcef.string_list_free(n_result);
            }
        }
    }
}
