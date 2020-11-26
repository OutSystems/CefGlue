using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CefGlue.Tests.Javascript
{
    public class JavascriptEvaluationTests : TestBase
    {
        class Person
        {
            public string Name = null;
            public int Age = 0;
        }

        protected async override Task ExtraSetup()
        {
            await Browser.LoadContent("<script></script>");
            await base.ExtraSetup();
        }

        [Test]
        public async Task NumberReturn()
        {
            var result = await EvaluateJavascript<int>("return 1+1;");
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task StringReturn()
        {
            const string Result = "this is a test";
            var result = await EvaluateJavascript<string>($"return '{Result}';");
            Assert.AreEqual(Result, result);
        }

        [Test]
        public async Task ListReturn()
        {
            var result = await EvaluateJavascript<int[]>("return [1, 2, 3];");
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
        }

        [Test]
        public async Task DynamicObjectReturn()
        {
            var result = await EvaluateJavascript<dynamic>("return { 'foo': 'foo-value', 'bar': 10, 'baz': [1, 2] }");
            Assert.AreEqual("foo-value", result.foo);
            Assert.AreEqual(10, result.bar);
            CollectionAssert.AreEqual(new [] { 1, 2 }, result.baz);
        }

        [Test]
        public async Task ObjectReturn()
        {
            var result = await EvaluateJavascript<Person>("return { 'Name': 'cef', 'Age': 10 }");
            Assert.AreEqual("cef", result.Name);
            Assert.AreEqual(10, result.Age);
        }

        [Test]
        public void ExceptionThrown()
        {
            const string ExceptionMessage = "ups";
            var exception = Assert.ThrowsAsync<Exception>(async () => await EvaluateJavascript<string>($"throw new Error('{ExceptionMessage}')"));
            StringAssert.Contains(ExceptionMessage, exception.Message);
        }

        [Test]
        public void CancelledOnTimeout()
        {
            var timeout = TimeSpan.FromMilliseconds(500);
            Assert.ThrowsAsync<TaskCanceledException>(async () => await EvaluateJavascript<string>($"var start = new Date(); while((new Date() - start) < ({timeout.TotalMilliseconds} + 200));", timeout));
        }

        [Test]
        public async Task NotCancelledBeforeTimeout()
        {
            var timeout = TimeSpan.FromMilliseconds(500);
            var result = await EvaluateJavascript<int>($"return 1;", timeout);
            Assert.AreEqual(1, result);
        }
    }
}
