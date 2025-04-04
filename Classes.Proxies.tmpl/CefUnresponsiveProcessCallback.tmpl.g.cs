namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface for asynchronous handling of an unresponsive process.
    /// </summary>
    public sealed unsafe partial class CefUnresponsiveProcessCallback
    {
        /// <summary>
        /// Reset the timeout for the unresponsive process.
        /// </summary>
        public void Wait()
        {
            throw new NotImplementedException(); // TODO: CefUnresponsiveProcessCallback.Wait
        }
        
        /// <summary>
        /// Terminate the unresponsive process.
        /// </summary>
        public void Terminate()
        {
            throw new NotImplementedException(); // TODO: CefUnresponsiveProcessCallback.Terminate
        }
        
    }
}
