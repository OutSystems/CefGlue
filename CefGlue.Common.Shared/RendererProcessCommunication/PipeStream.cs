using System;
using System.IO;
using System.Text;

namespace Xilium.CefGlue.Common.Shared.RendererProcessCommunication
{
    internal class PipeStream
    {
        private readonly Stream _stream;
        private readonly UnicodeEncoding _streamEncoding;

        public PipeStream(Stream stream)
        {
            _stream = stream;
            _streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            var length = ReadInt(_stream);
            var buffer = new byte[length];
            _stream.Read(buffer, 0, buffer.Length);

            return _streamEncoding.GetString(buffer);
        }

        public void WriteString(string message)
        {
            var buffer = _streamEncoding.GetBytes(message);
            WriteInt(_stream, buffer.Length);
            _stream.Write(buffer, 0, buffer.Length);
            _stream.Flush();
        }

        private static int ReadInt(Stream stream)
        {
            var valueInBytes = new byte[4];
            for (var i = 0; i < valueInBytes.Length; i++)
            {
                valueInBytes[i] = (byte)stream.ReadByte();
            }
            return BitConverter.ToInt32(valueInBytes, 0);
        }

        private static int WriteInt(Stream stream, int value)
        {
            var valueInBytes = BitConverter.GetBytes(value);
            foreach (var @byte in valueInBytes)
            {
                stream.WriteByte(@byte);
            }
            return valueInBytes.Length;
        }
    }
}
