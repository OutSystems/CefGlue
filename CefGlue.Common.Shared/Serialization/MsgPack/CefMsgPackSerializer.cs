using MessagePack;
using MessagePack.Resolvers;

namespace Xilium.CefGlue.Common.Shared.Serialization.MsgPack;

public class CefMsgPackSerializer : ISerializer
{
    public byte[] Serialize(object value)
    {
        return MessagePackSerializer.Serialize(value, ContractlessStandardResolverAllowPrivate.Options);
    }
}
