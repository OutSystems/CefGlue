﻿namespace Xilium.CefGlue
{
    using System;

    /// <summary>
    /// Focus sources.
    /// </summary>
    public enum CefFocusSource
    {
        /// <summary>
        /// The source is explicit navigation via the API (LoadURL(), etc).
        /// </summary>
        Navigation,

        /// <summary>
        /// The source is a system-generated focus event.
        /// </summary>
        System,
        
        NumValues,
    }
}
