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
        const string Date = "1995-12-17T03:24:00Z";

        protected const string ObjName = "nativeObj";

        protected NativeObject nativeObject;

        protected class Person
        {
            public string Name = null;
            public int Age = 0;
            public DateTime BirthDate = default;
            public byte[] Photo = default;
        }

        protected class NativeObject
        {
            private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

            public Task<object> ResultTask => _tcs.Task;
                
            public event Action TestCalled;

            public void Test()
            {
                TestCalled?.Invoke();
            }

            public void SetResult(object result)
            {
                _tcs.SetResult(result);
            }

            public void SetPersonResult(Person result)
            {
                _tcs.SetResult(result);
            }

            public event Action<object[]> MethodWithParamsCalled;

            public void MethodWithParams(string param1, int param2, DateTime param3, bool param4)
            {
                MethodWithParamsCalled?.Invoke(new object[] { param1, param2, param3, param4 });
            }

            public event Action<object[]> MethodWithObjectParamCalled;

            public void MethodWithObjectParam(Person param)
            {
                MethodWithObjectParamCalled?.Invoke(new object[] { param });
            }

            public object MethodWithNullReturn()
            {
                return null;
            }

            public event Func<int, Task<string>> AsyncMethodCalled;

            public Task<string> AsyncMethod(int arg)
            {
                if (AsyncMethodCalled != null) {
                    return AsyncMethodCalled(arg);
                }
                return Task.FromResult("this is the result");
            }

            public object MethodReturnException()
            {
                throw new Exception("error");
            }

            public Task AsyncMethodReturnException()
            {
                return Task.FromException(new Exception("error"));
            }
            
            public string MethodWithStringReturn()
            {
                return "this is the result";
            }

            public DateTime MethodWithDateTimeReturn()
            {
                return DateTime.Parse(Date);
            }

            public Person MethodWithObjectReturn()
            {
                return new Person() {Name = "John", Age = 30, BirthDate = DateTime.Parse(Date)};
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
        public async Task MethodIsCalled()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            nativeObject.TestCalled += () => taskCompletionSource.SetResult(true);
            Execute($"{ObjName}.test()");
            await taskCompletionSource.Task;
        }

        [Test]
        public async Task MethodParamsArePassed()
        {
            const string Arg1 = "test";
            const int Arg2 = 5;

            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithParamsCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithParams('{Arg1}', {Arg2}, new Date('{Date}'), true)");
            
            var result = await taskCompletionSource.Task;

            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(Arg1, result[0]);
            Assert.AreEqual(Arg2, result[1]);
            Assert.AreEqual(DateTime.Parse(Date), result[2]);
            Assert.AreEqual(true, result[3]);
        }

        [Test]
        public async Task MethodWithObjectParamIsPassed()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithObjectParamCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithObjectParam({{'Name': 'cef', 'Age': 10, 'BirthDate': new Date('{Date}') }})");

            var result = await taskCompletionSource.Task;
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(typeof(Person), result[0].GetType());

            var arg = (Person) result[0];
            Assert.AreEqual("cef", arg.Name);
            Assert.AreEqual(10, arg.Age);
            Assert.AreEqual(DateTime.Parse(Date), arg.BirthDate);
        }

        [Test]
        public async Task NativeObjectMethodNullResultIsReturned()
        {
            Execute($"{ObjName}.methodWithNullReturn().then(r => {ObjName}.setResult(r));");

            var result = await nativeObject.ResultTask;

            Assert.AreEqual(nativeObject.MethodWithNullReturn(), result);
        }

        [Test]
        public async Task NativeObjectMethodStringResultIsReturned()
        {
            Execute($"{ObjName}.methodWithStringReturn().then(r => {ObjName}.setResult(r));");

            var result = await nativeObject.ResultTask;

            Assert.AreEqual(nativeObject.MethodWithStringReturn(), result);
        }

        [Test]
        public async Task NativeObjectMethodDateTimeResultIsReturned()
        {
            Execute($"{ObjName}.methodWithDateTimeReturn().then(r => {ObjName}.setResult(r));");

            var result = await nativeObject.ResultTask;

            Assert.AreEqual(nativeObject.MethodWithDateTimeReturn(), result);
        }

        [Test]
        public async Task NativeObjectMethodObjectResultIsReturned()
        {
            Execute($"{ObjName}.methodWithObjectReturn().then(r => {ObjName}.setPersonResult(r));");

            var result = (Person) await nativeObject.ResultTask;

            var expected = nativeObject.MethodWithObjectReturn();
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Age, result.Age);
            Assert.AreEqual(expected.BirthDate, result.BirthDate);
            Assert.AreEqual(expected.Photo, result.Photo);
        }
        
        [Test]
        public void AsyncMethodsCanExecuteSimultaneously()
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
        public async Task AsyncMethodResultIsReturned()
        {
            Execute($"{ObjName}.asyncMethod(0).then(r => {ObjName}.setResult(r));");

            var result = await nativeObject.ResultTask; ;

            Assert.AreEqual(nativeObject.AsyncMethod(0).Result, result);
        }

        [Test]
        public async Task MethodExceptionIsReturned()
        {
            Execute($"{ObjName}.methodReturnException().catch(r => {ObjName}.setResult(r));");

            var result = await nativeObject.ResultTask;

            var msg = Assert.Throws<Exception>(() => nativeObject.MethodReturnException()).Message;
            Assert.AreEqual(msg, result);
        }

        [Test]
        public async Task AsyncMethodExceptionIsReturned()
        {
            Execute($"{ObjName}.asyncMethodReturnException().catch(r => {ObjName}.setResult(r));");

            var result = await nativeObject.ResultTask;

            Assert.AreEqual(nativeObject.AsyncMethodReturnException().Exception.Message, result);
        }
    }
}
