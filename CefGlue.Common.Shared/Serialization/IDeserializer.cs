using System;

namespace Xilium.CefGlue.Common.Shared.Serialization;

public interface IDeserializer
{
    object[] Deserialize(byte[] bytes, params Type[] targetTypes);

    TargetType Deserialize<TargetType>(byte[] bytes);
}
