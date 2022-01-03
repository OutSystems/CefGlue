using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectInteropTests : TestBase
    {
        protected const string ObjName = "nativeObj";

        protected NativeObject nativeObject;

        protected class Person
        {
            public string Name = null;
            public int Age = 0;
        }

        protected class NativeObject
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


            public event Func<int, Task<string>> AsyncMethodCalled;

            public Task<string> AsyncMethod(int arg)
            {
                if (AsyncMethodCalled != null) {
                    return AsyncMethodCalled(arg);
                }
                return Task.FromResult("this is the result");
            }
            
            public Task AsyncMethodReturnException()
            {
                return Task.FromException(new Exception("error"));
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
        
        protected override async Task ExtraSetup()
        {
            RegisterObject();
            await Load();
            await base.ExtraSetup();
        }

        protected virtual void RegisterObject()
        {
            nativeObject = new NativeObject();
            Browser.RegisterJavascriptObject(nativeObject, ObjName);
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
        
        [Test]
        public void NativeObjectAsyncMethodsCanExecuteSimultaneously()
        {
            const int CallsCount = 10;

            var waitHandle = new ManualResetEvent(false);
            var calls = new List<int>();
            nativeObject.AsyncMethodCalled += (arg) =>
            {
                calls.Add(arg);

                return Task.Run(() =>
                {
                    if (calls.Count < CallsCount)
                    {
                        waitHandle.WaitOne();
                    }
                    else
                    {
                        waitHandle.Set();
                    }

                    return "done";
                });
            };

            var script = string.Join("", Enumerable.Range(1, CallsCount).Select(i => $"{ObjName}.asyncMethod({i});"));
            Execute(script);

            waitHandle.WaitOne();
            Assert.AreEqual(CallsCount, calls.Count, "Number of calls dont match");
            for (var i = 1; i <= CallsCount; i++)
            {
                Assert.AreEqual(i, calls[i-1], "Call order failed");
            }
        }
        
        [Test]
        public async Task NativeObjectAsyncMethodResultIsReturned()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithParamsCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.asyncMethod(0).then(r => {ObjName}.methodWithParams(r, 0));");

            var result = await taskCompletionSource.Task;

            Assert.AreEqual(nativeObject.AsyncMethod(0).Result, result[0]);
        }
        
        [Test]
        public async Task NativeObjectAsyncMethodExceptionIsReturned()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithParamsCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.asyncMethodReturnException().catch(r => {ObjName}.methodWithParams(r, 0));");

            var result = await taskCompletionSource.Task;

            Assert.AreEqual(nativeObject.AsyncMethodReturnException().Exception.Message, result[0]);
        }
    }
}
