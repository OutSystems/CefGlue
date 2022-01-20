using System;
using NUnit.Framework;
using Xilium.CefGlue.Common.ObjectBinding;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectIntrospectionTests
    {
        class CustomObject
        {
            public event Action<object[]> Event;

            public void MethodWithParams(string param1, int param2) { }

            public static void StaticMethod() { }

            private void PrivateMethod() { }

            public string PublicProperty => "";
            
            private string PrivateProperty => "";
        }
        
        [Test]
        public void CRLObjectInstanceMethodsAreCaptured()
        {
            var members = NativeObjectAnalyser.AnalyseObjectMembers(new object());

            Assert.IsTrue(members.ContainsKey("toString"));
            Assert.IsTrue(members.ContainsKey("getHashCode"));
            Assert.IsTrue(members.ContainsKey("getType"));
            Assert.IsTrue(members.ContainsKey("equals"));
        }
        
        [Test]
        public void CustomObjectInstanceMethodsAreCaptured()
        {
            var members = NativeObjectAnalyser.AnalyseObjectMembers(new CustomObject());

            Assert.AreEqual(5, members.Count); // object members + 1 public method
            Assert.IsTrue(members.ContainsKey("methodWithParams"));
            Assert.IsTrue(members.ContainsKey("toString"));
            Assert.IsTrue(members.ContainsKey("getHashCode"));
            Assert.IsTrue(members.ContainsKey("getType"));
            Assert.IsTrue(members.ContainsKey("equals"));
        }
    }
}
