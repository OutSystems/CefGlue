using System;
using System.Threading.Tasks;
using CefGlue.Tests.Serialization;
using NUnit.Framework;
using Xilium.CefGlue.Common.ObjectBinding;
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

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void CLRObjectInstanceMethodsAreCaptured(MessageContextType messageContextType)
        {
            NativeObject nativeObject = new NativeObject(MessageContextTypeHelper.GetMessageContext(messageContextType), "object", new object());
            ObjectInfo objectInfo = nativeObject.ToObjectInfo();
            var members = objectInfo.Methods;

            Assert.AreEqual("object", objectInfo.Name);
            Assert.AreEqual(4, members.Length); // object members
            Assert.Contains(new MethodInfo("toString", 0), members);
            Assert.Contains(new MethodInfo("getHashCode", 0), members);
            Assert.Contains(new MethodInfo("getType", 0), members);
            Assert.Contains(new MethodInfo("equals", 1), members);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void CustomObjectInstanceMethodsAreCaptured(MessageContextType messageContextType)
        {
            NativeObject nativeObject = new NativeObject(MessageContextTypeHelper.GetMessageContext(messageContextType), "CustomObject", new CustomObject());
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
