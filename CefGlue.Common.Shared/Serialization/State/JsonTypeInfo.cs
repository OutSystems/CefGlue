using System;
using System.Collections.Generic;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal record JsonTypeInfo
    {
        public JsonTypeInfo(Type type)
        {
            ObjectType = type;
        }
        public Type ObjectType { get; set; }
        public Dictionary<string, TypeMemberInfo> TypeMembers { get; } = new Dictionary<string, TypeMemberInfo>();
        public TypeMethodInfo CollectionAddMethod { get; set; }
    }
}
