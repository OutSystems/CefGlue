namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class CefVersionMismatchException : Exception
    {
        public CefVersionMismatchException(string message)
            : base(message)
        {
        }
    }
}
