using System;

namespace Xilium.CefGlue.Common.Helpers
{
    internal class MessageReceivedEventArgs
    {
        public CefBrowser Browser { get; set; }
        public CefProcessId ProcessId { get; set; }
        public CefProcessMessage Message { get; set; }
    }
}
