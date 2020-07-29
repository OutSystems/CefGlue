using System;
using System.IO.Pipes;

namespace Xilium.CefGlue.Common.Shared.RendererProcessCommunication
{
    internal class PipeClient : IDisposable {

        private readonly NamedPipeClientStream _pipe;

        public PipeClient(string pipeName)
        {
            _pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.None);
            _pipe.Connect((int) TimeSpan.FromSeconds(10).TotalMilliseconds);
        }

        public void SendMessage(string message)
        {
            var stream = new PipeStream(_pipe);
            stream.WriteString(message);
        }

        public void Dispose()
        {
            _pipe.Close();
            _pipe.Dispose();
        }
    }
}
