namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface used to asynchronously continue a download.
    /// </summary>
    public sealed unsafe partial class CefBeforeDownloadCallback
    {
        /// <summary>
        /// Call to continue the download. Set |download_path| to the full file path
        /// for the download including the file name or leave blank to use the
        /// suggested name and the default temp directory. Set |show_dialog| to true
        /// if you do wish to show the default "Save As" dialog.
        /// </summary>
        public void Continue(cef_string_t* download_path, int show_dialog)
        {
            throw new NotImplementedException(); // TODO: CefBeforeDownloadCallback.Continue
        }
        
    }
}
