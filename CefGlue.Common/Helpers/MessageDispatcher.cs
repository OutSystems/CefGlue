using System;
using System.Collections.Generic;

namespace Xilium.CefGlue.Common.Helpers
{
    internal class MessageDispatcher
    {
        private readonly Dictionary<string, Action<MessageReceivedEventArgs>> _messageHandlers = new Dictionary<string, Action<MessageReceivedEventArgs>>();

        public void DispatchMessage(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            if (_messageHandlers.TryGetValue(message.Name, out var existingHandler))
            {
                existingHandler(new MessageReceivedEventArgs()
                {
                    Browser = browser,
                    ProcessId = sourceProcess,
                    Message = message
                });
            }
        }

        public void RegisterMessageHandler(string messageName, Action<MessageReceivedEventArgs> handler)
        {
            _messageHandlers.TryGetValue(messageName, out var existingHandler);
            _messageHandlers[messageName] = existingHandler + handler;
        }
    }
}
