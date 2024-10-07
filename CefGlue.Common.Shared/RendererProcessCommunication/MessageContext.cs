using System;
using Xilium.CefGlue.Common.Shared.Serialization;
using Xilium.CefGlue.Common.Shared.Serialization.Json;
using Xilium.CefGlue.Common.Shared.Serialization.MsgPack;

namespace Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

public class MessageContext
{
    public static readonly MessageContext DefaultJson = new MessageContext(new CefJsonSerializer(), new CefJsonDeserializer());
    public static readonly MessageContext DefaultMsgPack = new MessageContext(new CefMsgPackSerializer(), new CefMsgPackDeserializer());

    public ISerializer Serializer { get; }

    public IDeserializer Deserializer { get; }

    public MessageContext(ISerializer serializer, IDeserializer deserializer)
    {
        Serializer = serializer;
        Deserializer = deserializer;
    }

    public object[] Deserialize(byte[] bytes, params Type[] targetTypes) => Deserializer.Deserialize(bytes, targetTypes);

    public TargetType Deserialize<TargetType>(byte[] bytes) => Deserializer.Deserialize<TargetType>(bytes);

    public byte[] Serialize(object value) => Serializer.Serialize(value);
}
