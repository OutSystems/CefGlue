using System.Linq;
using System.Collections.Concurrent;
using Xilium.CefGlue.Common.RendererProcessCommunication;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectRegistry
    {
        private readonly CefBrowser _browser;
        private readonly ConcurrentDictionary<string, NativeObject> _registeredObjects = new ConcurrentDictionary<string, NativeObject>();

        public NativeObjectRegistry(CefBrowser browser)
        {
            _browser = browser;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns>True if the object was successfully registered, false if the object was already registered before.</returns>
        public bool Register(object obj, string name, JavascriptObjectMethodCallHandler methodHandler = null)
        {
            if (_registeredObjects.ContainsKey(name))
            {
                return false;
            }

            var objectMembers = NativeObjectAnalyser.AnalyseObjectMembers(obj);

            _registeredObjects.TryAdd(name, new NativeObject(obj, objectMembers));

            var message = new Messages.NativeObjectRegistrationRequest()
            {
                ObjectName = name,
                MethodsNames = objectMembers.Keys.ToArray()
            };

            _browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());

            return true;
        }

        public NativeObject Get(string name)
        {
            _registeredObjects.TryGetValue(name, out var obj);
            return obj;
        }
    }
}
