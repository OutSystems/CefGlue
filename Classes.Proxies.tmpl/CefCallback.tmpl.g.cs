namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Generic callback interface used for asynchronous continuation.
    /// </summary>
    public sealed unsafe partial class CefCallback
    {
        /// <summary>
        /// Continue processing.
        /// </summary>
        public void Continue()
        {
            throw new NotImplementedException(); // TODO: CefCallback.Continue
        }
        
        /// <summary>
        /// Cancel processing.
        /// </summary>
        public void Cancel()
        {
            throw new NotImplementedException(); // TODO: CefCallback.Cancel
        }
        
    }
}
