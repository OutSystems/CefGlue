namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface used for asynchronous continuation of quota requests.
    /// </summary>
    public sealed unsafe partial class CefQuotaCallback
    {
        /// <summary>
        /// Continue the quota request. If |allow| is true the request will be
        /// allowed. Otherwise, the request will be denied.
        /// </summary>
        public void Continue(bool allow)
        {
            cef_quota_callback_t.cont(_self, allow ? 1 : 0);
        }

        /// <summary>
        /// Cancel the quota request.
        /// </summary>
        public void Cancel()
        {
            cef_quota_callback_t.cancel(_self);
        }
    }
}
