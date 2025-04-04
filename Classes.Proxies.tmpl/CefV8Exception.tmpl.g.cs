namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a V8 exception. The methods of this class may be called
    /// on any render process thread.
    /// </summary>
    public sealed unsafe partial class CefV8Exception
    {
        /// <summary>
        /// Returns the exception message.
        /// </summary>
        public cef_string_userfree* GetMessage()
        {
            throw new NotImplementedException(); // TODO: CefV8Exception.GetMessage
        }
        
        /// <summary>
        /// Returns the line of source code that the exception occurred within.
        /// </summary>
        public cef_string_userfree* GetSourceLine()
        {
            throw new NotImplementedException(); // TODO: CefV8Exception.GetSourceLine
        }
        
        /// <summary>
        /// Returns the resource name for the script from where the function causing
        /// the error originates.
        /// </summary>
        public cef_string_userfree* GetScriptResourceName()
        {
            throw new NotImplementedException(); // TODO: CefV8Exception.GetScriptResourceName
        }
        
        /// <summary>
        /// Returns the 1-based number of the line where the error occurred or 0 if
        /// the line number is unknown.
        /// </summary>
        public int GetLineNumber()
        {
            throw new NotImplementedException(); // TODO: CefV8Exception.GetLineNumber
        }
        
        /// <summary>
        /// Returns the index within the script of the first character where the error
        /// occurred.
        /// </summary>
        public int GetStartPosition()
        {
            throw new NotImplementedException(); // TODO: CefV8Exception.GetStartPosition
        }
        
        /// <summary>
        /// Returns the index within the script of the last character where the error
        /// occurred.
        /// </summary>
        public int GetEndPosition()
        {
            throw new NotImplementedException(); // TODO: CefV8Exception.GetEndPosition
        }
        
        /// <summary>
        /// Returns the index within the line of the first character where the error
        /// occurred.
        /// </summary>
        public int GetStartColumn()
        {
            throw new NotImplementedException(); // TODO: CefV8Exception.GetStartColumn
        }
        
        /// <summary>
        /// Returns the index within the line of the last character where the error
        /// occurred.
        /// </summary>
        public int GetEndColumn()
        {
            throw new NotImplementedException(); // TODO: CefV8Exception.GetEndColumn
        }
        
    }
}
