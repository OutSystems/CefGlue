namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class representing a V8 stack frame. The methods of this class may only be
    /// called on the render process main thread.
    /// </summary>
    public sealed unsafe partial class CefV8StackFrame
    {
        /// <summary>
        /// Returns the name of the resource script that contains the function.
        /// </summary>
        public string ScriptName
        {
            get
            {
                var n_result = cef_v8stack_frame_t.get_script_name(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Returns the name of the resource script that contains the function or the
        /// sourceURL value if the script name is undefined and its source ends with
        /// a "//@ sourceURL=..." string.
        /// </summary>
        public string ScriptNameOrSourceUrl
        {
            get
            {
                var n_result = cef_v8stack_frame_t.get_script_name_or_source_url(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Returns the name of the function.
        /// </summary>
        public string FunctionName
        {
            get
            {
                var n_result = cef_v8stack_frame_t.get_function_name(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Returns the 1-based line number for the function call or 0 if unknown.
        /// </summary>
        public int LineNumber
        {
            get { return cef_v8stack_frame_t.get_line_number(_self); }
        }

        /// <summary>
        /// Returns the 1-based column offset on the line for the function call or 0 if
        /// unknown.
        /// </summary>
        public int Column
        {
            get { return cef_v8stack_frame_t.get_column(_self); }
        }

        /// <summary>
        /// Returns true if the function was compiled using eval().
        /// </summary>
        public bool IsEval
        {
            get { return cef_v8stack_frame_t.is_eval(_self) != 0; }
        }

        /// <summary>
        /// Returns true if the function was called as a constructor via "new".
        /// </summary>
        public bool IsConstructor
        {
            get { return cef_v8stack_frame_t.is_constructor(_self) != 0; }
        }
    }
}
