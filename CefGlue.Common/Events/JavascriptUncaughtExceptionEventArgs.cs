using System;

namespace Xilium.CefGlue.Common.Events
{
    public class JavascriptUncaughtExceptionEventArgs : EventArgs
	{
        public JavascriptUncaughtExceptionEventArgs(CefFrame frame, string message, JavascriptStackFrame[] stackFrames)
        {
            Frame = frame;
            Message = message;
            StackFrames = stackFrames;
        }

        public CefFrame Frame { get; }

        public string Message { get; }

        public JavascriptStackFrame[] StackFrames { get; }
    }

    public class JavascriptStackFrame
    {
        public JavascriptStackFrame(string functionName, string scriptNameOrSourceUrl, int column, int lineNumber)
        {
            FunctionName = functionName;
            ScriptNameOrSourceUrl = scriptNameOrSourceUrl;
            Column = column;
            LineNumber = lineNumber;
        }

        public string FunctionName { get; }
        public string ScriptNameOrSourceUrl { get; }
        public int Column { get; }
        public int LineNumber { get; }
    }
}
