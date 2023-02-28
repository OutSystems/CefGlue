using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Threading.Tasks;
using NUnit.Framework;
using Xilium.CefGlue.Common.Shared.Serialization;

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
            var stringEqualToDate = $"{DataMarkers.DateTimeMarker}{DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)}";
            result = await EvaluateJavascript<string>($"return '{stringEqualToDate}';");
            Assert.AreEqual(stringEqualToDate, result);
        }

        [Test]
        public async Task DateTimeReturn()
        {
            var expected = DateTime.Parse("2022-12-20T15:50:21.817Z");
            var result = await EvaluateJavascript<DateTime>($"return new Date('{expected.ToString("o", CultureInfo.InvariantCulture)}');");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task BinaryReturn()
        {
            var expected = new byte[byte.MaxValue + 1];
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                expected[i] = (byte)i;
            }
            var result = await EvaluateJavascript<byte[]>($"return new Uint8Array([{string.Join(",", expected)}]);");
            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task ListReturn()
        {
            var result = await EvaluateJavascript<int[]>("return [1, 2, 3];");
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
        }

        [Test]
        public async Task DictionaryCollectionReturn()
        {
            var result = await EvaluateJavascript<Dictionary<string, int>>("return {\"first\":1,\"second\":2,\"third\":3};");
            var expected = new Dictionary<string, int>()
            {
                { "first" , 1 },
                { "second" , 2 },
                { "third" , 3 }
            };
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public async Task NonGenericCollectionReturn()
        {
            var result = await EvaluateJavascript<Hashtable>("return {\"first\":1,\"second\":'second',\"third\":null};");
            var expected = new Hashtable()
            {
                { "first" , 1 },
                { "second" , "second" },
                { "third" , null }
            };
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public async Task ArrayListReturn()
        {
            var result = await EvaluateJavascript<ArrayList>("return ['first','second','third'];");
            var expected = new string[] { "first", "second", "third" };
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public async Task DynamicObjectReturn()
        {
            var result = await EvaluateJavascript<dynamic>("return { 'foo': 'foo-value', 'bar': 10, 'baz': [1, 2] }");
            Assert.IsInstanceOf<IDictionary<string, object>>(result);
            var obtainedDictionary = (IDictionary<string, object>)result;
            Assert.AreEqual("foo-value", obtainedDictionary["foo"]);
            Assert.AreEqual(10, obtainedDictionary["bar"]);
            Assert.IsInstanceOf<object[]>(obtainedDictionary["baz"]);
            CollectionAssert.AreEqual(new[] { 1, 2 }, (object[])obtainedDictionary["baz"]);
        }

        [Test]
        public async Task ExpandoObjectReturn()
        {
            var result = await (dynamic)EvaluateJavascript<ExpandoObject>("return { 'foo': 'foo-value', 'bar': 10, 'baz': [1, 2] }");
            Assert.AreEqual("foo-value", result.foo);
            Assert.AreEqual(10, result.bar);
            CollectionAssert.AreEqual(new[] { 1, 2 }, result.baz);
        }

        [Test]
        public async Task ObjectReturn()
        {
            var result = await EvaluateJavascript<Person>("return { 'Name': 'cef', 'Age': 10 }");
            Assert.AreEqual("cef", result.Name);
            Assert.AreEqual(10, result.Age);
        }

        [Test]
        public async Task CyclicObjectReturn()
        {
            var script =
                "const list = [1,null];" +
                "list[1] = list;" +
                "return list;";
            var result = await EvaluateJavascript<object[]>(script);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreSame(result, result[1]);
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

        [Test]
        public async Task DisposeBrowserShouldHandleInnerTaskCanceledExceptions()
        {
            // Arrange
            var evalTask = EvaluateJavascript<int>("const finishTime = new Date().getTime() + 10000; while(new Date().getTime() < finishTime); return 10;");

            // Act
            Browser.Dispose();

            // Assert
            try
            {
                var result = await evalTask;
                Assert.AreEqual(0, result);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Test]
        public async Task TryCatchScriptSucceeds()
        {
            var result = await EvaluateJavascript<int>("try { return 1 } catch (e) { return 'error' }");
            Assert.AreEqual(1, result);
        }

        [Test]
        public async Task SimpleExpressionScriptSucceeds()
        {
            var result = await EvaluateJavascript<int>("return 2+1");
            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task ScriptWithTrailingCommentSucceeds()
        {
            var result = await EvaluateJavascript<int>("return 1 //trailing comment");
            Assert.AreEqual(1, result);
        }
    }
}
