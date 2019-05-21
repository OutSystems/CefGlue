using System;
using System.Collections.Generic;
using System.Text;

namespace Xilium.CefGlue.Common
{
    internal class MessageReceivedEventArgs : EventArgs
    {
        public CefBrowser Browser { get; set; }
        public CefProcessId ProcessId { get; set; }
        public CefProcessMessage Message { get; set; }
    }
}
