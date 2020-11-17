using System;
using System.Collections.Generic;

namespace Xilium.CefGlue.Common.Shared.Helpers
{
    internal class MessageDispatcher
    {
        private readonly Dictionary<string, Action<MessageReceivedEventArgs>> _messageHandlers = new Dictionary<string, Action<MessageReceivedEventArgs>>();

        public void DispatchMessage(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (_messageHandlers.TryGetValue(message.Name, out var existingHandler))
            {
                existingHandler(new MessageReceivedEventArgs(browser, frame, sourceProcess, message));
            }
        }

        public void RegisterMessageHandler(string messageName, Action<MessageReceivedEventArgs> handler)
        {
            _messageHandlers.TryGetValue(messageName, out var existingHandler);
            _messageHandlers[messageName] = existingHandler + handler;
        }
    }
}
