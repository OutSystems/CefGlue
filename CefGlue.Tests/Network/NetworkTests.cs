using CefGlue.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Handlers;

namespace CefGlue.Tests.Network
{
    public class NetworkTests : TestBase
    {
        private class TestsRequestHandler : RequestHandler
        {
            private readonly TestsResourceRequestHandler _resourceRequestHandler;

            public TestsRequestHandler(Func<CefRequest, DefaultResourceHandler> resourceHandler)
            {
                _resourceRequestHandler = new TestsResourceRequestHandler(resourceHandler);
            }

            protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
            {
                return _resourceRequestHandler;
            }
        }

        private class TestsResourceRequestHandler : CefResourceRequestHandler
        {
            private readonly Func<CefRequest, DefaultResourceHandler> _resourceHandler;

            public TestsResourceRequestHandler(Func<CefRequest, DefaultResourceHandler> resourceHandler)
            {
                _resourceHandler = resourceHandler;
            }

            protected override CefCookieAccessFilter GetCookieAccessFilter(CefBrowser browser, CefFrame frame, CefRequest request)
            {
                return null;
            }

            protected override CefResourceHandler GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
            {
                return _resourceHandler(request);
            }
        }

        private class TestsResourceHandler : DefaultResourceHandler
        {
            public void Read()
            {
                Read(new MemoryStream(), 10, out var bytesRead, null);
            }
        }

        private class CustomStream : Stream
        {
            public event Action ReadStarted;

            public override bool CanRead => true;

            public override bool CanSeek => true;

            public override bool CanWrite => true;

            public override long Length => 1000;

            public override long Position { get; set; }

            public override void Flush() { }

            public override int Read(byte[] buffer, int offset, int count)
            {
                ReadStarted?.Invoke();
                Position += count;
                return count;
            }

            public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

            public override void SetLength(long value) { }

            public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
        }

        private class Response
        {
            public string AllowOrigin { get; set; }
            public string Status { get; set; }
            public string Data { get; set; }
            public string RedirectUrl { get; set; }
        }

        private Task<Response> GetResponse()
        {
            var taskCompletion = new TaskCompletionSource<Response>();

            Browser.ConsoleMessage += OnConsoleMessage;

            void OnConsoleMessage(object sender, ConsoleMessageEventArgs message)
            {
                Browser.ConsoleMessage -= OnConsoleMessage;
                var messageParts = message.Message.Split("|");
                if (messageParts.Length == 4)
                {
                    taskCompletion.SetResult(new Response()
                    {
                        AllowOrigin = messageParts[0],
                        Status = messageParts[1],
                        RedirectUrl = messageParts[2],
                        Data = messageParts[3]
                    });
                }
                else
                {
                    taskCompletion.SetResult(new Response()
                    {
                        Data = message.Message
                    });
                }
            }

            var script = 
                "fetch('https://tests/resource').then(response => {" +
                "   let result = [ response.headers.get('Access-Control-Allow-Origin'), response.status, response.url ];" +
                "   try {" +
                "       if (response.status === 200) {" +
                "           response.text().then(data => console.log(result.concat([ data ]).join('|')));" +
                "           return;" +
                "       }" +
                "   } catch {}" +
                "   console.log(result.concat([ '' ]).join('|'));" +
                "})";
            Browser.LoadContent("<html/>");
            EvaluateJavascript<int>(script);

            return taskCompletion.Task.ContinueWith(t =>
            {
                Browser.ConsoleMessage -= OnConsoleMessage;
                return t.Result;
            });
        }

        [Test]
        public async Task ResourceHandlerIsCalledWithStatusOk()
        {
            const string Data = "test";
            Browser.RequestHandler = new TestsRequestHandler(_ =>
            {
                var handler = new DefaultResourceHandler();
                handler.Response = StreamHelper.GetStream(Data);
                return handler;
            });

            var response = await GetResponse();

            Assert.AreEqual("*", response.AllowOrigin);
            Assert.AreEqual("200", response.Status);
            Assert.AreEqual(Data, response.Data);
        }

        [Test]
        public async Task ResourceHandlerIsCalledWithError()
        {
            Browser.RequestHandler = new TestsRequestHandler(_ => new DefaultResourceHandler());

            var response = await GetResponse();

            StringAssert.Contains("Error", response.Data);
        }

        [Test]
        public async Task ResourceHandlerWithRedirectUrl()
        {
            const string RedirectUrl = "http://test/otherurl";

            Browser.RequestHandler = new TestsRequestHandler(request =>
            {
                var handler = new DefaultResourceHandler();
                if (request.Url == RedirectUrl)
                {
                    handler.Response = StreamHelper.GetStream("ok");
                }
                else if (!request.Url.StartsWith("data:"))
                {
                    handler.RedirectUrl = RedirectUrl;
                }
                return handler;
            });

            var response = await GetResponse();

            StringAssert.Contains(RedirectUrl, response.RedirectUrl);
        }

        [Test]
        public async Task ResourceHandlerStreamsAreNotReadAtSameTime()
        {
            var checkPoints = new List<int>();

            var stream = new CustomStream();
            var handler1 = new TestsResourceHandler();
            var handler2 = new TestsResourceHandler();

            // both handlers share the same stream
            handler1.Response = stream;
            handler2.Response = stream;

            Task concurrentReadTask = null;

            void OnStreamReadStarted()
            {
                checkPoints.Add(2);
                stream.ReadStarted -= OnStreamReadStarted;

                var concurrentReadTaskStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                concurrentReadTask = Task.Run(() =>
                {
                    checkPoints.Add(3);
                    concurrentReadTaskStarted.SetResult(true);
                    handler2.Read();
                    checkPoints.Add(5);
                });
                concurrentReadTaskStarted.Task.Wait();
                // simulate read with 1s duration
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                checkPoints.Add(4);
            }

            stream.ReadStarted += () => Assert.AreEqual(0, stream.Position, "Streams Position on Reads must be 0");

            stream.ReadStarted += OnStreamReadStarted;

            checkPoints.Add(1);
            handler1.Read();

            Assert.IsNotNull(concurrentReadTask);
            await concurrentReadTask;
            checkPoints.Add(6);

            CollectionAssert.AreEqual(Enumerable.Range(1, 6), checkPoints);
        }
    }
}
