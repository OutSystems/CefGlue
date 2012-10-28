namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to receive trace notifications. The methods of this
    /// class will be called on the browser process UI thread.
    /// </summary>
    public abstract unsafe partial class CefTraceClient
    {
        private void on_trace_data_collected(cef_trace_client_t* self, byte* fragment, UIntPtr fragment_size)
        {
            CheckSelf(self);

            using (var stream = new UnmanagedMemoryStream(fragment, (long)fragment_size))
            {
                OnTraceDataCollected(stream);
            }
        }

        /// <summary>
        /// Called 0 or more times between CefBeginTracing and OnEndTracingComplete
        /// with a UTF8 JSON |fragment| of the specified |fragment_size|. Do not keep
        /// a reference to |fragment|.
        /// </summary>
        protected abstract void OnTraceDataCollected(Stream fragment);


        private void on_trace_buffer_percent_full_reply(cef_trace_client_t* self, float percent_full)
        {
            CheckSelf(self);

            OnTraceBufferPercentFullReply(percent_full);
        }

        /// <summary>
        /// Called in response to CefGetTraceBufferPercentFullAsync.
        /// </summary>
        protected abstract void OnTraceBufferPercentFullReply(float percentFull);


        private void on_end_tracing_complete(cef_trace_client_t* self)
        {
            CheckSelf(self);

            OnEndTracingComplete();
        }

        /// <summary>
        /// Called after all processes have sent their trace data.
        /// </summary>
        protected abstract void OnEndTracingComplete();
    }
}
