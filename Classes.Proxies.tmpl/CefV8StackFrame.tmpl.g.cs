namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a V8 stack frame handle. V8 handles can only be accessed
    /// from the thread on which they are created. Valid threads for creating a V8
    /// handle include the render process main thread (TID_RENDERER) and WebWorker
    /// threads. A task runner for posting tasks on the associated thread can be
    /// retrieved via the CefV8Context::GetTaskRunner() method.
    /// </summary>
    public sealed unsafe partial class CefV8StackFrame
    {
        /// <summary>
        /// Returns true if the underlying handle is valid and it can be accessed on
        /// the current thread. Do not call any other methods if this method returns
        /// false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefV8StackFrame.IsValid
        }
        
        /// <summary>
        /// Returns the name of the resource script that contains the function.
        /// </summary>
        public cef_string_userfree* GetScriptName()
        {
            throw new NotImplementedException(); // TODO: CefV8StackFrame.GetScriptName
        }
        
        /// <summary>
        /// Returns the name of the resource script that contains the function or the
        /// sourceURL value if the script name is undefined and its source ends with
        /// a "//@ sourceURL=..." string.
        /// </summary>
        public cef_string_userfree* GetScriptNameOrSourceURL()
        {
            throw new NotImplementedException(); // TODO: CefV8StackFrame.GetScriptNameOrSourceURL
        }
        
        /// <summary>
        /// Returns the name of the function.
        /// </summary>
        public cef_string_userfree* GetFunctionName()
        {
            throw new NotImplementedException(); // TODO: CefV8StackFrame.GetFunctionName
        }
        
        /// <summary>
        /// Returns the 1-based line number for the function call or 0 if unknown.
        /// </summary>
        public int GetLineNumber()
        {
            throw new NotImplementedException(); // TODO: CefV8StackFrame.GetLineNumber
        }
        
        /// <summary>
        /// Returns the 1-based column offset on the line for the function call or 0
        /// if unknown.
        /// </summary>
        public int GetColumn()
        {
            throw new NotImplementedException(); // TODO: CefV8StackFrame.GetColumn
        }
        
        /// <summary>
        /// Returns true if the function was compiled using eval().
        /// </summary>
        public int IsEval()
        {
            throw new NotImplementedException(); // TODO: CefV8StackFrame.IsEval
        }
        
        /// <summary>
        /// Returns true if the function was called as a constructor via "new".
        /// </summary>
        public int IsConstructor()
        {
            throw new NotImplementedException(); // TODO: CefV8StackFrame.IsConstructor
        }
        
    }
}
