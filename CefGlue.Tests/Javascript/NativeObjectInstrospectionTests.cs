using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Xilium.CefGlue.Common.ObjectBinding;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectIntrospectionTests
    {
        class CustomObject
        {
            public event Action<object[]> MethodWithParamsCalled;

            public void MethodWithParams(string param1, int param2)
            {
                MethodWithParamsCalled?.Invoke(new object[] { param1, param2 });
            }

            public Task<string> AsyncGenericMethod()
            {
                return Task.FromResult(string.Empty);
            }
            
            public Task AsyncMethod()
            {
                return Task.CompletedTask;
            }
            
            public static void StaticMethod()
            {
            }

            private void PrivateMethod()
            {
            }

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

            Assert.AreEqual(7, members.Count); // object members + 3 public methods
            Assert.IsTrue(members.ContainsKey("toString"));
            Assert.IsTrue(members.ContainsKey("getHashCode"));
            Assert.IsTrue(members.ContainsKey("getType"));
            Assert.IsTrue(members.ContainsKey("equals"));
            
            members.TryGetValue("methodWithParams", out var method);
            Assert.IsNotNull(method);
            Assert.IsFalse(method.IsAsync);
            
            members.TryGetValue("asyncMethod", out var asyncMethod);
            Assert.IsNotNull(asyncMethod);
            Assert.IsTrue(asyncMethod.IsAsync);
            
            members.TryGetValue("asyncGenericMethod", out var asyncGenericMethod);
            Assert.IsNotNull(asyncGenericMethod);
            Assert.IsTrue(asyncGenericMethod.IsAsync);
        }
    }
}
