namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class representing a V8 stack trace. The methods of this class may only be
    /// called on the render process main thread.
    /// </summary>
    public sealed unsafe partial class CefV8StackTrace
    {
        /// <summary>
        /// Returns the stack trace for the currently active context. |frame_limit| is
        /// the maximum number of frames that will be captured.
        /// </summary>
        public static CefV8StackTrace GetCurrent(int frameLimit)
        {
            return CefV8StackTrace.FromNative(
                cef_v8stack_trace_t.get_current(frameLimit)
                );
        }

        /// <summary>
        /// Returns the number of stack frames.
        /// </summary>
        public int FrameCount
        {
            get { return cef_v8stack_trace_t.get_frame_count(_self); }
        }

        /// <summary>
        /// Returns the stack frame at the specified 0-based index.
        /// </summary>
        public CefV8StackFrame GetFrame(int index)
        {
            return CefV8StackFrame.FromNative(
                cef_v8stack_trace_t.get_frame(_self, index)
                );
        }
    }
}
