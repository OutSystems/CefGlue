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

        protected class CyclicObject
        {
            public string Name = null;
            public CyclicObject Parent;
            public CyclicObject Child;
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

            public event Action<object[]> MethodWithFixedAndOptionalParamsCalled;

            public void MethodWithFixedAndOptionalParams(int p1, params string[] args)
            {
                MethodWithFixedAndOptionalParamsCalled?.Invoke(new object[] { p1, args });
            }

            public event Action<string[]> MethodWithOptionalParamsCalled;

            public void MethodWithOptionalParams(params string[] args)
            {
                MethodWithOptionalParamsCalled?.Invoke(args);
            }

            public event Action<object[]> MethodWithStringAndObjectsParamsCalled;

            public void MethodWithStringAndObjectsParams(string param1, Person person1, Person person2, Person person3)
            {
                MethodWithStringAndObjectsParamsCalled?.Invoke(new object[] { param1, person1, person2, person3 });
            }

            public event Action<object[]> MethodWithStringParamCalled;

            public void MethodWithStringParam(string param1)
            {
                MethodWithStringParamCalled?.Invoke(new object[] { param1 });
            }

            public event Action<object[]> MethodWithObjectParamCalled;

            public void MethodWithObjectParam(Person param)
            {
                MethodWithObjectParamCalled?.Invoke(new object[] { param });
            }

            public event Action<object[]> MethodWithCyclicObjectParamCalled;

            public void MethodWithCyclicObjectParam(CyclicObject param)
            {
                MethodWithCyclicObjectParamCalled?.Invoke(new object[] { param });
            }

            public event Action<object[]> MethodWithObjectsArrayParamCalled;

            public void MethodWithObjectsArrayParam(object[] param)
            {
                MethodWithObjectsArrayParamCalled?.Invoke(param);
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
        public async Task MethodFixedAndOptionalParamsArePassed()
        {
            const int P1 = 7;
            const string Arg1 = "test";
            const string Arg2 = "s2";

            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithFixedAndOptionalParamsCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithFixedAndOptionalParams({P1}, '{Arg1}', '{Arg2}')");

            var result = await taskCompletionSource.Task;

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(P1, result[0]);
            Assert.IsInstanceOf<string[]>(result[1]);
            var optionalParams = (string[])result[1];
            Assert.AreEqual(Arg1, optionalParams[0]);
            Assert.AreEqual(Arg2, optionalParams[1]);
        }

        [Test]
        public async Task MethodOptionalParamsArePassed()
        {
            const string Arg1 = "test";
            const string Arg2 = "s2";

            var taskCompletionSource = new TaskCompletionSource<string[]>();
            nativeObject.MethodWithOptionalParamsCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithOptionalParams('{Arg1}', '{Arg2}')");

            var result = await taskCompletionSource.Task;

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(Arg1, result[0]);
            Assert.AreEqual(Arg2, result[1]);
        }

        [Test]
        public async Task MethodEmptyStringParamIsPassed()
        {
            const string Arg1 = "";

            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithStringParamCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithStringParam('{Arg1}')");

            var result = await taskCompletionSource.Task;

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(Arg1, result[0]);
        }

        [Test]
        public async Task MethodNullStringParamIsPassed()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithStringParamCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithStringParam(null)");

            var result = await taskCompletionSource.Task;

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(null, result[0]);
        }

        [Test]
        public async Task MethodWithObjectParamIsPassed()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithObjectParamCalled += (args) => taskCompletionSource.SetResult(args);

            Execute($"{ObjName}.methodWithObjectParam({{'Name': 'cef', 'Age': 10, 'BirthDate': new Date('{Date}') }})");

            var result = await taskCompletionSource.Task;
            Assert.AreEqual(1, result.Length);
            Assert.IsInstanceOf<Person>(result[0]);

            var arg = (Person) result[0];
            Assert.AreEqual("cef", arg.Name);
            Assert.AreEqual(10, arg.Age);
            Assert.AreEqual(DateTime.Parse(Date), arg.BirthDate);
        }

        [Test]
        public async Task MethodWithCyclicObjectParamIsPassed()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithCyclicObjectParamCalled += (args) => taskCompletionSource.SetResult(args);

            var script = @$"const parent={{Name:'parent1', Parent:null,Child:null}};
                            const child={{Name:'child1',Parent:parent,Child:null}};
                            parent.Child=child;
                            {ObjName}.methodWithCyclicObjectParam(parent);";
            Execute(script);

            var result = await taskCompletionSource.Task;
            Assert.AreEqual(1, result.Length);
            Assert.IsInstanceOf<CyclicObject>(result[0]);

            var arg = (CyclicObject)result[0];
            Assert.AreEqual("parent1", arg.Name);
            Assert.NotNull(arg.Child);
            Assert.AreEqual("child1", arg.Child.Name);
            Assert.NotNull(arg.Child.Parent);
            Assert.AreSame(arg, arg.Child.Parent);
        }

        [Test]
        public async Task MethodWithStringAndObjectsParamsIsPassed()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithStringAndObjectsParamsCalled += (args) => taskCompletionSource.SetResult(args);

            var script = @$"const person1 = {{'Name': 'person1', 'Age': 10, 'BirthDate': new Date('{Date}') }};
                            const person2 = {{'Name': 'person2', 'Age': 15, 'BirthDate': new Date('{Date}') }};
                            {ObjName}.methodWithStringAndObjectsParams('stringParam', person1, person2, person1);";
            Execute(script);

            var result = await taskCompletionSource.Task;
            Assert.AreEqual(4, result.Length);
            Assert.IsInstanceOf<string>(result[0]);
            Assert.IsInstanceOf<Person>(result[1]);
            Assert.IsInstanceOf<Person>(result[2]);
            Assert.IsInstanceOf<Person>(result[3]);

            Assert.AreEqual("stringParam", result[0]);
            Assert.AreEqual("person1", ((Person)result[1]).Name);
            Assert.AreEqual("person2", ((Person)result[2]).Name);
            Assert.AreSame(result[1], result[3]);
        }

        [Test]
        public async Task MethodWithObjectsArrayParamIsPassed()
        {
            var taskCompletionSource = new TaskCompletionSource<object[]>();
            nativeObject.MethodWithObjectsArrayParamCalled += (args) => taskCompletionSource.SetResult(args);

            var script = @$"const intsArray=[1,2,3];
                            const objs=[null, 'text', 5, {{Name: 'plainObjName'}}, intsArray, intsArray];
                            {ObjName}.methodWithObjectsArrayParam(objs);";
            Execute(script);

            var result = await taskCompletionSource.Task;
            Assert.IsInstanceOf<object[]>(result);
            Assert.AreEqual(6, result.Length);
            Assert.IsNull(result[0]);
            Assert.AreEqual("text", result[1]);
            Assert.AreEqual(5, result[2]);
            Assert.IsInstanceOf<IDictionary<string, object>>(result[3]);
            var dict = (IDictionary<string, object>)result[3];
            Assert.AreEqual(1, dict.Count);
            Assert.AreEqual("Name", dict.Keys.Single());
            Assert.AreEqual("plainObjName", dict.Values.Single());
            Assert.IsInstanceOf<object[]>(result[4]);
            var arr = (object[])result[4];
            Assert.AreEqual(3, arr.Length);
            Assert.AreEqual(2, arr[1]);
            Assert.AreSame(result[4], result[5]);
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
            //Setup
            // Act
            Execute($"{ObjName}.methodWithDateTimeReturn().then(r => {ObjName}.setResult(r));");

            var result = await nativeObject.ResultTask;

            // Assert
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
