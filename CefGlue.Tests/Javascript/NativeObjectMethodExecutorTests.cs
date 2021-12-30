using System.Collections;
using NUnit.Framework;
using Xilium.CefGlue.Common.ObjectBinding;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectMethodExecutorTests
    {
        class NativeTestObject
        {
            public object MethodWithParams(string param1, int param2)
            {
                return new object[] { param1, param2 };
            }

            public string MethodWithReturn()
            {
                return "this is the result";
            }

            public object[] MethodWithOptionalParams(params string[] optionalParams)
            {
                return optionalParams;
            }

            public object[] MethodWithFixedAndOptionalParams(string fixedParam, params int[] optionalParams)
            {
                return new object[] { fixedParam, optionalParams };
            }
        }

        private NativeTestObject nativeTestObject = new NativeTestObject();
        private NativeObject nativeObjectInfo;

        [OneTimeSetUp]
        protected void Setup()
        {
            var objectMembers = NativeObjectAnalyser.AnalyseObjectMembers(nativeTestObject);
            nativeObjectInfo = new NativeObject("test", nativeTestObject, objectMembers);
        }

        private object ExecuteMethod(string name, object[] args)
        {
            var method = nativeObjectInfo.GetNativeMethod(name);
            return method.Execute(nativeTestObject, args);
        }

        [Test]
        public void MethodWithReturnIsExecuted()
        {
            var result = ExecuteMethod("methodWithReturn", new object[0]);
            Assert.AreEqual(nativeTestObject.MethodWithReturn(), result);
        }

        [Test]
        public void MethodWithParamsIsExecuted()
        {
            const string Arg1 = "arg1";
            const int Arg2 = 2;
            var result = (object[]) ExecuteMethod("methodWithParams", new object[] { Arg1, Arg2 } );

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(Arg1, result[0]);
            Assert.AreEqual(Arg2, result[1]);
        }

        [Test]
        public void MethodWithOptionalParams()
        {
            const string Arg1 = "arg1";
            const string Arg2 = "arg2";
            var result = (object[])ExecuteMethod("methodWithOptionalParams", new object[] { Arg1, Arg2 });

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(Arg1, result[0]);
            Assert.AreEqual(Arg2, result[1]);

            result = (object[])ExecuteMethod("methodWithOptionalParams", new object[0]);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void MethodWithFixedAndOptionalParams()
        {
            const string Arg1 = "arg1";
            var arg2 = new int[] { 1, 2 , 3 };
            var result = (object[])ExecuteMethod("methodWithFixedAndOptionalParams", new object[] { Arg1, 1, 2, 3 });

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(Arg1, result[0]);
            CollectionAssert.AreEqual(arg2, (IEnumerable) result[1]);

            result = (object[])ExecuteMethod("methodWithFixedAndOptionalParams", new object[] { Arg1 });

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(Arg1, result[0]);
            Assert.AreEqual(0, ((int[]) result[1]).Length);
        }
    }
}
