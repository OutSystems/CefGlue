namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class that encapsulates a V8 context handle. The methods of this class may
    /// only be called on the render process main thread.
    /// </summary>
    public sealed unsafe partial class CefV8Context
    {
        /// <summary>
        /// Returns the current (top) context object in the V8 context stack.
        /// </summary>
        public static CefV8Context GetCurrentContext()
        {
            return CefV8Context.FromNative(
                cef_v8context_t.get_current_context()
                );
        }

        /// <summary>
        /// Returns the entered (bottom) context object in the V8 context stack.
        /// </summary>
        public static CefV8Context GetEnteredContext()
        {
            return CefV8Context.FromNative(
                cef_v8context_t.get_entered_context()
                );
        }

        /// <summary>
        /// Returns true if V8 is currently inside a context.
        /// </summary>
        public static bool InContext
        {
            get { return cef_v8context_t.in_context() != 0; }
        }

        /// <summary>
        /// Returns the browser for this context.
        /// </summary>
        public CefBrowser GetBrowser()
        {
            return CefBrowser.FromNative(
                cef_v8context_t.get_browser(_self)
                );
        }

        /// <summary>
        /// Returns the frame for this context.
        /// </summary>
        public CefFrame GetFrame()
        {
            return CefFrame.FromNative(
                cef_v8context_t.get_frame(_self)
                );
        }

        /// <summary>
        /// Returns the global object for this context. The context must be entered
        /// before calling this method.
        /// </summary>
        public CefV8Value GetGlobal()
        {
            return CefV8Value.FromNative(
                cef_v8context_t.get_global(_self)
                );
        }

        /// <summary>
        /// Enter this context. A context must be explicitly entered before creating a
        /// V8 Object, Array, Function or Date asynchronously. Exit() must be called
        /// the same number of times as Enter() before releasing this context. V8
        /// objects belong to the context in which they are created. Returns true if
        /// the scope was entered successfully.
        /// </summary>
        public bool Enter()
        {
            return cef_v8context_t.enter(_self) != 0;
        }

        /// <summary>
        /// Exit this context. Call this method only after calling Enter(). Returns
        /// true if the scope was exited successfully.
        /// </summary>
        public bool Exit()
        {
            return cef_v8context_t.exit(_self) != 0;
        }

        /// <summary>
        /// Returns true if this object is pointing to the same handle as |that|
        /// object.
        /// </summary>
        public bool IsSame(CefV8Context that)
        {
            if (that == null) return false;
            return cef_v8context_t.is_same(_self, that.ToNative()) != 0;
        }

        /// <summary>
        /// Evaluates the specified JavaScript code using this context's global object.
        /// On success |retval| will be set to the return value, if any, and the
        /// function will return true. On failure |exception| will be set to the
        /// exception, if any, and the function will return false.
        /// </summary>
        public bool TryEval(string code, out CefV8Value returnValue, out CefV8Exception exception)
        {
            bool result;
            cef_v8value_t* n_retval = null;
            cef_v8exception_t* n_exception = null;

            fixed (char* code_str = code)
            {
                var n_code = new cef_string_t(code_str, code != null ? code.Length : 0);
                result = cef_v8context_t.eval(_self, &n_code, &n_retval, &n_exception) != 0;
            }

            returnValue = n_retval != null ? CefV8Value.FromNative(n_retval) : null;
            exception = n_exception != null ? CefV8Exception.FromNative(n_exception) : null;

            return result;
        }
    }
}
