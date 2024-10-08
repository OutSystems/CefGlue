using System;

using Xilium.CefGlue.Common.Shared.Serialization;
using Xilium.CefGlue.Common.Shared.Serialization.Json;
using Xilium.CefGlue.Common.Shared.Serialization.MsgPack;

namespace Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

public class Messaging
{
    public static readonly Messaging Json = new Messaging("Json", new CefJsonSerializer(), new CefJsonDeserializer());
    public static readonly Messaging MsgPack = new Messaging("MsgPack", new CefMsgPackSerializer(), new CefMsgPackDeserializer());

    public string Id { get; }

    public ISerializer Serializer { get; }

    public IDeserializer Deserializer { get; }

    public Messaging(string id, ISerializer serializer, IDeserializer deserializer)
    {
        Id = id;
        Serializer = serializer;
        Deserializer = deserializer;
    }

    public object[] Deserialize(byte[] bytes, params Type[] targetTypes) => Deserializer.Deserialize(bytes, targetTypes);

    public TargetType Deserialize<TargetType>(byte[] bytes) => Deserializer.Deserialize<TargetType>(bytes);

    public byte[] Serialize(object value) => Serializer.Serialize(value);
}
