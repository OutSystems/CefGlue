using System;
using System.IO;
using System.Xml.Serialization;

namespace Xilium.CefGlue.Common.Shared.RendererProcessCommunication
{
    [Serializable]
    public class SerializableException {

        public string ExceptionType { get; set; }
        public string Message { get; set; }
        public string StackTrace{ get; set; }

        public string SerializeToString()
        {
            var serializer = new XmlSerializer(typeof(SerializableException));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, this);
                return writer.ToString();
            }
        }

        public static SerializableException DeserializeFromString(string content)
        {
            var serializer = new XmlSerializer(typeof(SerializableException));
            using (var reader = new StringReader(content))
            {
                return (SerializableException) serializer.Deserialize(reader);
            }
        }
    }
}
