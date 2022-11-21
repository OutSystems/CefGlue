﻿namespace Xilium.CefGlue.Wrapper
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Used to configure the query router. The same values must be passed to both
    /// CefMessageRouterBrowserSide and CefMessageRouterRendererSide. If using multiple
    /// router pairs make sure to choose values that do not conflict.
    /// </summary>
    public sealed class CefMessageRouterConfig
    {
        public CefMessageRouterConfig()
        {
            JSQueryFunction = "cefQuery";
            JSCancelFunction = "cefQueryCancel";
            MessageSizeThreshold = CefMessageRouter.ResponseSizeThreshold;
        }

        /// <summary>
        /// Name of the JavaScript function that will be added to the 'window' object
        /// for sending a query. The default value is "cefQuery".
        /// </summary>
        public string JSQueryFunction { get; set; }

        /// <summary>
        /// Name of the JavaScript function that will be added to the 'window' object
        /// for canceling a pending query. The default value is "cefQueryCancel".
        /// </summary>
        public string JSCancelFunction { get; set; }

        /// <summary>
        /// Messages of size (in bytes) larger than this threshold will be sent via
        /// shared memory region.
        /// </summary>
        /// <remarks>Not implemented and has no effect.</remarks>
        public int MessageSizeThreshold { get; set; }

        // Validate configuration settings.
        internal bool Validate()
        {
            // Must specify function names.
            if (string.IsNullOrEmpty(JSQueryFunction) || string.IsNullOrEmpty(JSCancelFunction))
            {
                return false;
            }

            if (MessageSizeThreshold < 0)
            {
                return false;
            }

            return true;
        }
    }
}
