namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface for asynchronous continuation of print dialog requests.
    /// </summary>
    public sealed unsafe partial class CefPrintDialogCallback
    {
        /// <summary>
        /// Continue printing with the specified |settings|.
        /// </summary>
        public void Continue(cef_print_settings_t* settings)
        {
            throw new NotImplementedException(); // TODO: CefPrintDialogCallback.Continue
        }
        
        /// <summary>
        /// Cancel the printing.
        /// </summary>
        public void Cancel()
        {
            throw new NotImplementedException(); // TODO: CefPrintDialogCallback.Cancel
        }
        
    }
}
