using System;
using System.Collections.Generic;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectRegistry : IDisposable
    {
        private CefBrowser _browser;
        private readonly Dictionary<string, NativeObject> _registeredObjects = new Dictionary<string, NativeObject>();
        private readonly object _registrationSyncRoot = new object();

        public Messaging DefaultMessaging { get; } = Messaging.MsgPack;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="methodHandler"></param>
        /// <param name="messaging"></param>
        /// <returns>True if the object was successfully registered, false if the object was already registered before.</returns>
        public bool Register(object obj, string name, MethodCallHandler methodHandler = null, Messaging messaging = null)
        {
            if (_registeredObjects.ContainsKey(name))
            {
                return false;
            }

            messaging = messaging ?? DefaultMessaging;

            var nativeObj = new NativeObject(messaging, name, obj, methodHandler);

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
                    SendRegistrationMessage(nativeObj, messaging.Id);
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
                    SendRegistrationMessage(obj, null);
                }
            }
        }

        public NativeObject Get(string name)
        {
            _registeredObjects.TryGetValue(name, out var obj);
            return obj;
        }

        private void SendRegistrationMessage(NativeObject obj, string serializer)
        {
            var message = new Messages.NativeObjectRegistrationRequest()
            {
                ObjectInfo = obj.ToObjectInfo(),
                Messaging = serializer,
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
