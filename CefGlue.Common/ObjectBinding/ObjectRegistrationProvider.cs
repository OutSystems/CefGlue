using System.Collections.Concurrent;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class ObjectRegistrationProvider
    {
        private static volatile int objTrackIdCounter;

        private readonly CefBrowser _browser;
        private readonly ConcurrentDictionary<string, object> _registeredObjects = new ConcurrentDictionary<string, object>();

        public ObjectRegistrationProvider(CefBrowser browser)
        {
            _browser = browser;
        }

        public void Register(object obj)
        {
            var trackId = objTrackIdCounter++;

            var message = new Messages.JsObjectRegistrationRequest()
            {
                ObjectTrackId = trackId
            };
            _browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());
        }
    }
}
