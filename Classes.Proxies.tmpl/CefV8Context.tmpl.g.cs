namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a V8 context handle. V8 handles can only be accessed from
    /// the thread on which they are created. Valid threads for creating a V8 handle
    /// include the render process main thread (TID_RENDERER) and WebWorker threads.
    /// A task runner for posting tasks on the associated thread can be retrieved
    /// via the CefV8Context::GetTaskRunner() method.
    /// </summary>
    public sealed unsafe partial class CefV8Context
    {
        /// <summary>
        /// Returns the current (top) context object in the V8 context stack.
        /// </summary>
        public static cef_v8context_t* GetCurrentContext()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.GetCurrentContext
        }
        
        /// <summary>
        /// Returns the entered (bottom) context object in the V8 context stack.
        /// </summary>
        public static cef_v8context_t* GetEnteredContext()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.GetEnteredContext
        }
        
        /// <summary>
        /// Returns true if V8 is currently inside a context.
        /// </summary>
        public static int InContext()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.InContext
        }
        
        /// <summary>
        /// Returns the task runner associated with this context. V8 handles can only
        /// be accessed from the thread on which they are created. This method can be
        /// called on any render process thread.
        /// </summary>
        public cef_task_runner_t* GetTaskRunner()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.GetTaskRunner
        }
        
        /// <summary>
        /// Returns true if the underlying handle is valid and it can be accessed on
        /// the current thread. Do not call any other methods if this method returns
        /// false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.IsValid
        }
        
        /// <summary>
        /// Returns the browser for this context. This method will return an empty
        /// reference for WebWorker contexts.
        /// </summary>
        public cef_browser_t* GetBrowser()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.GetBrowser
        }
        
        /// <summary>
        /// Returns the frame for this context. This method will return an empty
        /// reference for WebWorker contexts.
        /// </summary>
        public cef_frame_t* GetFrame()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.GetFrame
        }
        
        /// <summary>
        /// Returns the global object for this context. The context must be entered
        /// before calling this method.
        /// </summary>
        public cef_v8value_t* GetGlobal()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.GetGlobal
        }
        
        /// <summary>
        /// Enter this context. A context must be explicitly entered before creating a
        /// V8 Object, Array, Function or Date asynchronously. Exit() must be called
        /// the same number of times as Enter() before releasing this context. V8
        /// objects belong to the context in which they are created. Returns true if
        /// the scope was entered successfully.
        /// </summary>
        public int Enter()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.Enter
        }
        
        /// <summary>
        /// Exit this context. Call this method only after calling Enter(). Returns
        /// true if the scope was exited successfully.
        /// </summary>
        public int Exit()
        {
            throw new NotImplementedException(); // TODO: CefV8Context.Exit
        }
        
        /// <summary>
        /// Returns true if this object is pointing to the same handle as |that|
        /// object.
        /// </summary>
        public int IsSame(cef_v8context_t* that)
        {
            throw new NotImplementedException(); // TODO: CefV8Context.IsSame
        }
        
        /// <summary>
        /// Execute a string of JavaScript code in this V8 context. The |script_url|
        /// parameter is the URL where the script in question can be found, if any.
        /// The |start_line| parameter is the base line number to use for error
        /// reporting. On success |retval| will be set to the return value, if any,
        /// and the function will return true. On failure |exception| will be set to
        /// the exception, if any, and the function will return false.
        /// </summary>
        public int Eval(cef_string_t* code, cef_string_t* script_url, int start_line, cef_v8value_t** retval, cef_v8exception_t** exception)
        {
            throw new NotImplementedException(); // TODO: CefV8Context.Eval
        }
        
    }
}
