using System;
using System.Collections.Generic;
using System.Linq;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectRegistry : IDisposable
    {
        private CefBrowser _browser;
        private readonly Dictionary<string, NativeObject> _registeredObjects = new Dictionary<string, NativeObject>();
        private readonly object _registrationSyncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns>True if the object was successfully registered, false if the object was already registered before.</returns>
        public bool Register(object obj, string name, MethodCallHandler methodHandler = null)
        {
            if (_registeredObjects.ContainsKey(name))
            {
                return false;
            }
            
            var nativeObj = new NativeObject(name, obj, methodHandler);

            lock (_registrationSyncRoot)
            {
                if (_registeredObjects.ContainsKey(name))
                {
                    // check gain, might have been registered meanwhile
                    return false;
                }

                _registeredObjects.Add(name, nativeObj);
                
                if (_browser != null)
                {
                    SendRegistrationMessage(nativeObj);
                }

                return true;
            }
        }

        public void Unregister(string name)
        {
            lock (_registrationSyncRoot)
            {
                _registeredObjects.Remove(name);

                if (_browser != null)
                {
                    var message = new Messages.NativeObjectUnregistrationRequest()
                    {
                        ObjectName = name,
                    };

                    var cefMessage = message.ToCefProcessMessage();
                    // TODO target main frame?
                    _browser.GetMainFrame().SendProcessMessage(CefProcessId.Renderer, cefMessage);
                }
            }
        }

        public void SetBrowser(CefBrowser browser)
        {
            lock (_registrationSyncRoot)
            {
                _browser = browser;
                foreach (var obj in _registeredObjects.Values)
                {
                    SendRegistrationMessage(obj);
                }
            }
        }

        public NativeObject Get(string name)
        {
            _registeredObjects.TryGetValue(name, out var obj);
            return obj;
        }

        private void SendRegistrationMessage(NativeObject obj)
        {
            var message = new Messages.NativeObjectRegistrationRequest()
            {
                ObjectName = obj.Name,
                MethodsNames = obj.MethodsNames.ToArray()
            };

            var cefMessage = message.ToCefProcessMessage();
            // TODO target main frame?
            _browser.GetMainFrame().SendProcessMessage(CefProcessId.Renderer, cefMessage);
        }

        public void Dispose()
        {
            lock (_registrationSyncRoot)
            {
                _registeredObjects.Clear();
            }

            _browser = null;
        }
    }
}
