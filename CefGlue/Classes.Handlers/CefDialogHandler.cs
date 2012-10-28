namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to handle dialog events. The methods of this class
    /// will be called on the browser process UI thread.
    /// </summary>
    public abstract unsafe partial class CefDialogHandler
    {
        private int on_file_dialog(cef_dialog_handler_t* self, cef_browser_t* browser, CefFileDialogMode mode, cef_string_t* title, cef_string_t* default_file_name, cef_string_list* accept_types, cef_file_dialog_callback_t* callback)
        {
            CheckSelf(self);

            var mBrowser = CefBrowser.FromNative(browser);
            var mTitle = cef_string_t.ToString(title);
            var mDefaultFileName = cef_string_t.ToString(default_file_name);
            var mAcceptTypes = cef_string_list.ToArray(accept_types);
            var mCallback = CefFileDialogCallback.FromNative(callback);

            var result = OnFileDialog(mBrowser, mode, mTitle, mDefaultFileName, mAcceptTypes, mCallback);

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called to run a file chooser dialog. |mode| represents the type of dialog
        /// to display. |title| to the title to be used for the dialog and may be empty
        /// to show the default title ("Open" or "Save" depending on the mode).
        /// |default_file_name| is the default file name to select in the dialog.
        /// |accept_types| is a list of valid lower-cased MIME types or file extensions
        /// specified in an input element and is used to restrict selectable files to
        /// such types. To display a custom dialog return true and execute |callback|
        /// either inline or at a later time. To display the default dialog return
        /// false.
        /// </summary>
        protected virtual bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFileName, string[] acceptTypes, CefFileDialogCallback callback)
        {
            return false;
        }
    }
}
