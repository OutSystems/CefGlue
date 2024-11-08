using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Xilium.CefGlue.Common.ObjectBinding;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectIntrospectionTests
    {
        class CustomObject
        {
            private Action<object[]> _event;
            public event Action<object[]> Event
            {
                add => _event += value;
                remove => _event -= value;
            }

            public void MethodWithParams(string param1, int param2) { }

            public Task AsyncMethod() => Task.CompletedTask;
            
            public static void StaticMethod() { }

            private void PrivateMethod() { }

            public string PublicProperty => "";
            
            private string PrivateProperty => "";
        }

        private static Messaging GetMessaging(MessagingType messagingType) => messagingType switch
        {
            MessagingType.Json => Messaging.Json,
            MessagingType.MsgPack => Messaging.MsgPack,
            _ => throw new ArgumentException($"Invalid MessagingType argument: {messagingType}")
        };

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void CLRObjectInstanceMethodsAreCaptured(MessagingType messagingType)
        {
            Messaging messaging = GetMessaging(messagingType);
            NativeObject nativeObject = new NativeObject(messaging, "object", new object());
            ObjectInfo objectInfo = nativeObject.ToObjectInfo();
            var members = objectInfo.Methods;

            Assert.AreEqual("object", objectInfo.Name);
            Assert.AreEqual(4, members.Length); // object members
            Assert.Contains(new MethodInfo("toString", 0), members);
            Assert.Contains(new MethodInfo("getHashCode", 0), members);
            Assert.Contains(new MethodInfo("getType", 0), members);
            Assert.Contains(new MethodInfo("equals", 1), members);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void CustomObjectInstanceMethodsAreCaptured(MessagingType messagingType)
        {
            Messaging messaging = GetMessaging(messagingType);
            NativeObject nativeObject = new NativeObject(messaging, "CustomObject", new CustomObject());
            ObjectInfo objectInfo = nativeObject.ToObjectInfo();
            var members = objectInfo.Methods;

            Assert.AreEqual("customObject", objectInfo.Name);
            Assert.AreEqual(6, members.Length); // object members + 2 public methods
            Assert.Contains(new MethodInfo("toString", 0), members);
            Assert.Contains(new MethodInfo("getHashCode", 0), members);
            Assert.Contains(new MethodInfo("getType", 0), members);
            Assert.Contains(new MethodInfo("equals", 1), members);
            Assert.Contains(new MethodInfo("methodWithParams", 2), members);
            Assert.Contains(new MethodInfo("asyncMethod", 0), members);
        }
    }
}
