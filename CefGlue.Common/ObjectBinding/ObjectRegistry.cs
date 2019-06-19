using System.Collections.Concurrent;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class ObjectRegistry
    {
        private static volatile int objTrackIdCounter;

        private readonly CefBrowser _browser;
        private readonly ConcurrentDictionary<string, object> _registeredObjects = new ConcurrentDictionary<string, object>();

        public ObjectRegistry(CefBrowser browser)
        {
            _browser = browser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns>True if the object was successfully registered, false if the object was already registered before.</returns>
        public bool Register(object obj, string name)
        {
            if (!_registeredObjects.TryAdd(name, obj))
            {
                return false;
            }

            var trackId = objTrackIdCounter++;
            var objectMembers = ObjectAnalyser.AnalyseObjectMembers(obj);
            
            var message = new Messages.JsObjectRegistrationRequest()
            {
                ObjectTrackId = trackId,
                ObjectName = name,
                MethodsNames = objectMembers
            };

            _browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());

            return true;
        }
    }
}
