using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectInteropTests : TestBase
    {
        const string ObjName = "nativeObj";

        private NativeObject nativeObject;

        class Person
        {
            public string Name = null;
            public int Age = 0;
        }

        class NativeObject
        {
            public event Action TestCalled;

            public void Test()
            {
                TestCalled?.Invoke();
            }

            public event Action<object[]> MethodWithParamsCalled;

            public void MethodWithParams(string param1, int param2)
            {
                MethodWithParamsCalled?.Invoke(new object[] { param1, param2 });
            }

            public event Action<object[]> MethodWithObjectParamCalled;

            public void MethodWithObjectParam(Person param)
            {
                MethodWithObjectParamCalled?.Invoke(new object[] { param });
            }

            public string MethodWithReturn()
            {
                return "this is the result";
            }
        }

        private Task Load()
        {
            return Browser.LoadContent($"<script></script>");
        }

        private void Execute(string script)
        {
            Browser.ExecuteJavaScript("(function() { " + script + " })()");
        }

        protected async override Task ExtraSetup()
        {
            nativeObject = new NativeObject();
            Browser.RegisterJavascriptObject(nativeObject, ObjName);
            await Load();
            await base.ExtraSetup();
        }

        [Test]
        public async Task ObjectIsRegistered()
        {
            var nativeObjectDefined = await EvaluateJavascript<bool>($"return window['{ObjName}'] !== null");
            Assert.IsTrue(nativeObjectDefined);

            var unregisteredObjectDefined = await EvaluateJavascript<bool>($"return window['foo'] === null");
            Assert.IsFalse(unregisteredObjectDefined);
        }

        [Test]
        public async Task NativeObjectMethodIsCalled()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            nativeObject.TestCalled += () => taskCompletionSource.SetResult(true);
            Execute($"{ObjName}.test()");
            await taskCompletionSource.Task;
        }

        [Test]
        public async Task NativeObjectMethodParamsArePassed()
        {
            const string Arg1 = "test";
            const int Arg2 = 5;

            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithParamsCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithParams('{Arg1}', {Arg2})");
            
            var result = await taskCompletionSource.Task;

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(Arg1, result[0]);
            Assert.AreEqual(Arg2, result[1]);
        }

        [Test]
        public async Task NativeObjectMethodWithObjectParamIsPassed()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithObjectParamCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithObjectParam({{'Name': 'cef', 'Age': 10 }})");

            var result = await taskCompletionSource.Task;
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(typeof(Person), result[0].GetType());

            var arg = (Person) result[0];
            Assert.AreEqual("cef", arg.Name);
            Assert.AreEqual(10, arg.Age);
        }

        [Test]
        public async Task NativeObjectMethodResultIsReturned()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithParamsCalled += (args) => taskCompletionSource.SetResult(args);
            
            Execute($"{ObjName}.methodWithReturn().then(r => {ObjName}.methodWithParams(r, 0));");

            var result = await taskCompletionSource.Task;

            Assert.AreEqual(nativeObject.MethodWithReturn(), result[0]);
        }
    }
}
