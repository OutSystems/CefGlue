using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

namespace Xilium.CefGlue.Common.Handlers
{
    public class DefaultResourceHandler : CefResourceHandler
    {
        private Stream _responseStream;
        private long _responseStreamReadPosition = -1;

        public DefaultResourceHandler()
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Allow-Origin
            // Workaround for requests from different scheme (e.g. requests from https made to custom-scheme)
            Headers.Add("Access-Control-Allow-Origin", "*");
        }

        /// <summary>
        /// Gets or sets the response mime type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Response returned.
        /// </summary>
        public Stream Response 
        { 
            get => _responseStream;
            set 
            {                
                if (_responseStreamReadPosition > -1)
                {
                    throw new Exception($"Cannot set {nameof(Response)} Stream after request handling started");
                }
                _responseStream = value;
            } 
        }

        /// <summary>
        /// Gets or sets the response error code. When set, the response is ignored.
        /// </summary>
        public CefErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the response status code.
        /// </summary>
        public int Status { get; set; } = 200;

        /// <summary>
        /// Gets or sets the status text.
        /// </summary>
        public string StatusText { get; set; } = "OK";

        /// <summary>
        /// Get all response header fields.
        /// </summary>
        public NameValueCollection Headers { get; set; } = new NameValueCollection();

        /// <summary>
        /// The url to redirect to;
        /// </summary>
        public string RedirectUrl { get; set; }

        protected override void Cancel()
        {
            // nothing to do
        }

        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string outRedirectUrl)
        {
            outRedirectUrl = null;

            var headers = Headers;
            if (headers != null)
            {
                response.SetHeaderMap(headers);
            }

            var errorCode = ErrorCode;
            if (errorCode != CefErrorCode.None)
            {
                response.Error = errorCode;
                responseLength = 0;
                return;
            }

            responseLength = -1;

            response.MimeType = MimeType ?? "text/html";
            response.Status = Status;
            response.StatusText = StatusText;

            var redirectUrl = RedirectUrl;
            if (redirectUrl != null)
            {
                outRedirectUrl = redirectUrl;
                return;
            }

            var responseStream = _responseStream;
            if (responseStream?.CanSeek == true)
            {
                // attempt to infer the length
                responseLength = responseStream.Length;
            }
        }

        protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback)
        {
            var fashion = ProcessRequestAsync(request, callback);
            switch (fashion)
            {
                case RequestHandlingFashion.Continue:
                    handleRequest = true;
                    return true;

                case RequestHandlingFashion.ContinueAsync:
                    handleRequest = false;
                    return true;

                default:
                    handleRequest = true;
                    return false;
            }
        }

        protected virtual RequestHandlingFashion ProcessRequestAsync(CefRequest request, CefCallback callback)
        {
            return RequestHandlingFashion.Continue;
        }

        protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback)
        {
            InitializeStreamPositionIfNeeded();

            var responseStream = _responseStream;
            if (responseStream?.CanSeek != true)
            {
                bytesSkipped = -2; // ERR_FAILED
                return false;
            }

            bytesSkipped = bytesToSkip;
            lock (responseStream)
            {
                _responseStreamReadPosition += bytesToSkip;
            }
            return true;
        }

        protected override bool Read(Stream outResponse, int bytesToRead, out int bytesRead, CefResourceReadCallback callback)
        {
            callback?.Dispose();

            InitializeStreamPositionIfNeeded();

            var responseStream = _responseStream;
            if (responseStream == null)
            {
                bytesRead = -2; // ERR_FAILED
                return false;
            }

            var buffer = new byte[bytesToRead];

            // lock response stream because it can be shared with other resource handlers
            lock (responseStream)
            {
                if (responseStream.Position != _responseStreamReadPosition)
                {
                    if (!responseStream.CanSeek)
                    {
                        bytesRead = -2; // ERR_FAILED
                        return false;
                    }
                    responseStream.Position = _responseStreamReadPosition;
                }
                
                bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                _responseStreamReadPosition = responseStream.Position;
            }

            if (bytesRead == 0)
            {
                return false;
            }

            outResponse.Write(buffer, 0, bytesRead);

            return bytesRead > 0;
        }

        private void InitializeStreamPositionIfNeeded() => Interlocked.CompareExchange(ref _responseStreamReadPosition, 0, -1); // mark as initialized if not yet
    }
}
