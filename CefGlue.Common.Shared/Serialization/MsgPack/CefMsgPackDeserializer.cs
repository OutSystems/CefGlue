using MessagePack;
using MessagePack.Resolvers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xilium.CefGlue.Common.Shared.Serialization.MsgPack;
public class CefMsgPackDeserializer : IDeserializer
{
    static Type[] TupleTypes = [
        typeof(ValueTuple<>),
        typeof(ValueTuple<,>),
        typeof(ValueTuple<,,>),
        typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>),
        typeof(ValueTuple<,,,,,>),
        typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>),
    ];

    public object[] Deserialize(byte[] bytes, params Type[] targetTypes)
    {
        IEnumerable<object> iterateTuple(ITuple tuple)
        {
            for (int i = 0; i < tuple.Length; i++)
            {
                yield return tuple[i];
            }
        }

        Type createTupleType(Type[] argumentTypes)
        {
            // Validate input and handle edge cases (e.g., empty argumentTypes)
            if (argumentTypes == null || argumentTypes.Length == 0)
                throw new ArgumentException("At least one argument type is required.");

            // Create a generic Tuple type with the specified argument types
            Type tupleType = TupleTypes[argumentTypes.Length - 1].MakeGenericType(argumentTypes);

            return tupleType;
        }

        Type type = createTupleType(targetTypes);
        //var json = MessagePackSerializer.ConvertToJson(bytes);
        return iterateTuple((ITuple)MessagePackSerializer.Deserialize(type, bytes, ContractlessStandardResolverAllowPrivate.Options)).ToArray();
    }

    public TargetType Deserialize<TargetType>(byte[] bytes)
    {
        return MessagePackSerializer.Deserialize<TargetType>(bytes, ContractlessStandardResolverAllowPrivate.Options);
    }
}
