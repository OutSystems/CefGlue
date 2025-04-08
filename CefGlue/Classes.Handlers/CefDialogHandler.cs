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
        private int on_file_dialog(cef_dialog_handler_t* self, cef_browser_t* browser, CefFileDialogMode mode, cef_string_t* title, cef_string_t* default_file_path, cef_string_list* accept_filters, cef_string_list* accept_extensions, cef_string_list* accept_descriptions, cef_file_dialog_callback_t* callback)
        {
            CheckSelf(self);

            var mBrowser = CefBrowser.FromNative(browser);
            var mTitle = cef_string_t.ToString(title);
            var mDefaultFilePath = cef_string_t.ToString(default_file_path);
            var mAcceptFilters = cef_string_list.ToArray(accept_filters);
            var mAcceptExtensions = cef_string_list.ToArray(accept_extensions);
            var mAcceptDescriptions = cef_string_list.ToArray(accept_descriptions);

            var mCallback = CefFileDialogCallback.FromNative(callback);

            var result = OnFileDialog(mBrowser, mode, mTitle, mDefaultFilePath, mAcceptFilters, mAcceptExtensions, mAcceptDescriptions, mCallback);
            return result ? 1 : 0;
        }

        /// <summary>
        /// Called to run a file chooser dialog. |mode| represents the type of dialog
        /// to display. |title| to the title to be used for the dialog and may be
        /// empty to show the default title ("Open" or "Save" depending on the mode).
        /// |default_file_path| is the path with optional directory and/or file name
        /// component that should be initially selected in the dialog.
        /// |accept_filters| are used to restrict the selectable file types and may be
        /// any combination of valid lower-cased MIME types (e.g. "text/*" or
        /// "image/*") and individual file extensions (e.g. ".txt" or ".png").
        /// |accept_extensions| provides the semicolon-delimited expansion of MIME
        /// types to file extensions (if known, or empty string otherwise).
        /// |accept_descriptions| provides the descriptions for MIME types (if known,
        /// or empty string otherwise). For example, the "image/*" mime type might
        /// have extensions ".png;.jpg;.bmp;..." and description "Image Files".
        /// |accept_filters|, |accept_extensions| and |accept_descriptions| will all
        /// be the same size. To display a custom dialog return true and execute
        /// |callback| either inline or at a later time. To display the default dialog
        /// return false. If this method returns false it may be called an additional
        /// time for the same dialog (both before and after MIME type expansion).
        /// </summary>
        protected virtual bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, string[] acceptFilters, string[] acceptExtensions, string[] acceptDescriptions, CefFileDialogCallback callback)
            => false;
    }
}
