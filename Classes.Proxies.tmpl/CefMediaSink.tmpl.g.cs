namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Represents a sink to which media can be routed. Instances of this object are
    /// retrieved via CefMediaObserver::OnSinks. The methods of this class may
    /// be called on any browser process thread unless otherwise indicated.
    /// </summary>
    public sealed unsafe partial class CefMediaSink
    {
        /// <summary>
        /// Returns the ID for this sink.
        /// </summary>
        public cef_string_userfree* GetId()
        {
            throw new NotImplementedException(); // TODO: CefMediaSink.GetId
        }
        
        /// <summary>
        /// Returns the name of this sink.
        /// </summary>
        public cef_string_userfree* GetName()
        {
            throw new NotImplementedException(); // TODO: CefMediaSink.GetName
        }
        
        /// <summary>
        /// Returns the icon type for this sink.
        /// </summary>
        public CefMediaSinkIconType GetIconType()
        {
            throw new NotImplementedException(); // TODO: CefMediaSink.GetIconType
        }
        
        /// <summary>
        /// Asynchronously retrieves device info.
        /// </summary>
        public void GetDeviceInfo(cef_media_sink_device_info_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefMediaSink.GetDeviceInfo
        }
        
        /// <summary>
        /// Returns true if this sink accepts content via Cast.
        /// </summary>
        public int IsCastSink()
        {
            throw new NotImplementedException(); // TODO: CefMediaSink.IsCastSink
        }
        
        /// <summary>
        /// Returns true if this sink accepts content via DIAL.
        /// </summary>
        public int IsDialSink()
        {
            throw new NotImplementedException(); // TODO: CefMediaSink.IsDialSink
        }
        
        /// <summary>
        /// Returns true if this sink is compatible with |source|.
        /// </summary>
        public int IsCompatibleWith(cef_media_source_t* source)
        {
            throw new NotImplementedException(); // TODO: CefMediaSink.IsCompatibleWith
        }
        
    }
}
