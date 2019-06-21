using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Xilium.CefGlue.Common
{
    internal static class CefValueSerialization
    {
        public static object DeserializeCefValue(CefValue value)
        {
            switch (value.GetValueType())
            {
                case CefValueType.Binary:
                    var binaryDate = BitConverter.ToInt64(value.GetBinary().ToArray(), 0);
                    return DateTime.FromBinary(binaryDate);

                case CefValueType.Bool:
                    return value.GetBool();

                case CefValueType.Dictionary:
                    IDictionary<string, object> dictionary = new ExpandoObject();
                    var v8Dictionary = value.GetDictionary();
                    var keys = v8Dictionary.GetKeys();
                    foreach (var key in keys)
                    {
                        dictionary[key] = DeserializeCefValue(v8Dictionary.GetValue(key));
                    }
                    return dictionary;

                case CefValueType.Double:
                    return value.GetDouble();

                case CefValueType.List:
                    return DeserializeCefList<object>(value.GetList());

                case CefValueType.Int:
                    return value.GetInt();

                case CefValueType.String:
                    return value.GetString();

                case CefValueType.Null:
                    return null;
            }

            return null;
        }

        public static ListElementType[] DeserializeCefList<ListElementType>(CefListValue cefList)
        {
            var array = new ListElementType[cefList.Count];
            for (var i = 0; i < cefList.Count; i++)
            {
                array[i] = (ListElementType)DeserializeCefValue(cefList.GetValue(i));
            }
            return array;
        }
    }
}
