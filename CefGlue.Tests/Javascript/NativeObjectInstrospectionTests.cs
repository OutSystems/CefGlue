using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Xilium.CefGlue.Common.ObjectBinding;

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
        
        [Test]
        public void CRLObjectInstanceMethodsAreCaptured()
        {
            var members = new HashSet<string>(new NativeObject("", new object()).MethodsNames);

            Assert.IsTrue(members.Contains("toString"));
            Assert.IsTrue(members.Contains("getHashCode"));
            Assert.IsTrue(members.Contains("getType"));
            Assert.IsTrue(members.Contains("equals"));
        }
        
        [Test]
        public void CustomObjectInstanceMethodsAreCaptured()
        {
            var members = new HashSet<string>(new NativeObject("", new CustomObject()).MethodsNames);

            Assert.AreEqual(6, members.Count); // object members + 2 public methods
            Assert.IsTrue(members.Contains("toString"));
            Assert.IsTrue(members.Contains("getHashCode"));
            Assert.IsTrue(members.Contains("getType"));
            Assert.IsTrue(members.Contains("equals"));
            Assert.IsTrue(members.Contains("methodWithParams"));
            Assert.IsTrue(members.Contains("asyncMethod"));
        }
    }
}
