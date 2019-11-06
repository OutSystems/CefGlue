namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Text;
    using System.Threading;

    internal sealed class DumpRequestResourceHandler : CefResourceHandler
    {
        private static int _requestNo;

        private byte[] responseData;
        private int pos;

        protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback)
        {
            // Backwards compatibility. ProcessRequest will be called.
            callback.Dispose();
            handleRequest = false;
            return false;
        }

        protected override bool ProcessRequest(CefRequest request, CefCallback callback)
        {
            var requestNo = Interlocked.Increment(ref _requestNo);

            var response = new StringBuilder();

            response.AppendFormat("<pre>\n");
            response.AppendFormat("Requests processed by DemoAppResourceHandler: {0}\n", requestNo);

            response.AppendFormat("Method: {0}\n", request.Method);
            response.AppendFormat("URL: {0}\n", request.Url);

            response.AppendLine();
            response.AppendLine("Headers:");
            var headers = request.GetHeaderMap();
            foreach (string key in headers)
            {
                foreach (var value in headers.GetValues(key))
                {
                    response.AppendFormat("{0}: {1}\n", key, value);
                }
            }
            response.AppendLine();

            response.AppendFormat("</pre>\n");

            responseData = Encoding.UTF8.GetBytes(response.ToString());

            callback.Continue();
            return true;
        }

        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            response.MimeType = "text/html";
            response.Status = 200;
            response.StatusText = "OK, hello from handler!";

            var headers = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            headers.Add("Cache-Control", "private");
            response.SetHeaderMap(headers);

            responseLength = responseData.LongLength;
            redirectUrl = null;
        }

        protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback)
        {
            bytesSkipped = (long)CefErrorCode.Failed;
            return false;
        }

        protected override bool Read(IntPtr dataOut, int bytesToRead, out int bytesRead, CefResourceReadCallback callback)
        {
            // Backwards compatibility. ReadResponse will be called.
            callback.Dispose();
            bytesRead = -1;
            return false;
        }

        protected override bool ReadResponse(Stream response, int bytesToRead, out int bytesRead, CefCallback callback)
        {
            if (bytesToRead == 0 || pos >= responseData.Length)
            {
                bytesRead = 0;
                return false;
            }
            else
            {
                var br = Math.Min(responseData.Length - pos, bytesToRead);
                response.Write(responseData, pos, br);
                pos += br;
                bytesRead = br;
                return true;
            }
        }

        protected override void Cancel()
        {
        }
    }
}
