using System.IO;
using System.Text;

namespace CefGlue.Tests.Helpers
{
    class StreamHelper
    {
        public static Stream GetStream(string content) => new MemoryStream(Encoding.ASCII.GetBytes(content));
    }
}
