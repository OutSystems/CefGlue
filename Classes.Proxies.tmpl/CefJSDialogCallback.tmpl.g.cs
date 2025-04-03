namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface used for asynchronous continuation of JavaScript dialog
    /// requests.
    /// </summary>
    public sealed unsafe partial class CefJSDialogCallback
    {
        /// <summary>
        /// Continue the JS dialog request. Set |success| to true if the OK button was
        /// pressed. The |user_input| value should be specified for prompt dialogs.
        /// </summary>
        public void Continue(int success, cef_string_t* user_input)
        {
            throw new NotImplementedException(); // TODO: CefJSDialogCallback.Continue
        }
        
    }
}
