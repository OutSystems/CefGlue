using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Xilium.CefGlue.Common.Handlers
{
    public class DefaultResourceHandler : CefResourceHandler
    {

        /// <summary>
        /// Gets or sets the response mime type.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Response returned.
        /// </summary>
        public Stream Response { get; set; }

        /// <summary>
        /// Gets or sets the response error code. When set, the response is ignored.
        /// </summary>
        public CefErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the response status code.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the status text.
        /// </summary>
        public string StatusText { get; set; }

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
            Response = null;
        }

        protected override bool CanGetCookie(CefCookie cookie)
        {
            return true;   
        }

        protected override bool CanSetCookie(CefCookie cookie)
        {
            return true;
        }

        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            redirectUrl = null;

            if (ErrorCode != CefErrorCode.None)
            {
                response.Error = ErrorCode;
                responseLength = 0;
            }
            else
            {
                responseLength = -1;

                if (RedirectUrl != null)
                {
                    redirectUrl = RedirectUrl;
                    return;
                }

                response.MimeType = MimeType ?? "text/html";
                response.Status = Status;
                response.StatusText = StatusText;
                if (Headers != null)
                {
                    response.SetHeaderMap(Headers);
                }
                
                // attempt to infer the length
                if (Response != null && Response.CanSeek)
                {
                    responseLength = Response.Length;
                    Response.Position = 0; // reset the stream
                }
            }
        }

        protected override bool ProcessRequest(CefRequest request, CefCallback callback)
        {
            callback.Continue();
            return true;
        }

        protected override bool ReadResponse(Stream response, int bytesToRead, out int bytesRead, CefCallback callback)
        {
            callback.Dispose();

            if (Response == null)
            {
                bytesRead = 0;
                return false;
            }

            var buffer = new byte[response.Length];
            bytesRead = Response.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                return false;
            }

            response.Write(buffer, 0, buffer.Length);

            return bytesRead > 0;
        }
    }
}
