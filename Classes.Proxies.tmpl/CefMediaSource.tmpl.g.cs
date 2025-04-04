namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Represents a source from which media can be routed. Instances of this object
    /// are retrieved via CefMediaRouter::GetSource. The methods of this class may
    /// be called on any browser process thread unless otherwise indicated.
    /// </summary>
    public sealed unsafe partial class CefMediaSource
    {
        /// <summary>
        /// Returns the ID (media source URN or URL) for this source.
        /// </summary>
        public cef_string_userfree* GetId()
        {
            throw new NotImplementedException(); // TODO: CefMediaSource.GetId
        }
        
        /// <summary>
        /// Returns true if this source outputs its content via Cast.
        /// </summary>
        public int IsCastSource()
        {
            throw new NotImplementedException(); // TODO: CefMediaSource.IsCastSource
        }
        
        /// <summary>
        /// Returns true if this source outputs its content via DIAL.
        /// </summary>
        public int IsDialSource()
        {
            throw new NotImplementedException(); // TODO: CefMediaSource.IsDialSource
        }
        
    }
}
