using System;
using System.Collections.Generic;
using System.Linq;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.BrowserProcess.Serialization
{
    internal static class V8ValueSerialization
    {
        public static void SerializeV8Object(CefV8Value obj, CefValueWrapper cefValue)
        {
            SerializeV8Object(obj, cefValue, new Stack<CefV8Value>());
        }

        private static void SerializeV8Object(CefV8Value obj, CefValueWrapper cefValue, Stack<CefV8Value> visitedObjects)
        {
            if (visitedObjects.Any(o => o.IsSame(obj)))
            {
                throw new InvalidOperationException("Cycle found in result");
            }

            visitedObjects.Push(obj);

            if (obj.IsNull || obj.IsUndefined)
            {
                cefValue.SetNull();
            }
            else if (obj.IsBool)
            {
                cefValue.SetBool(obj.GetBoolValue());
            }
            else if (obj.IsInt)
            {
                cefValue.SetInt(obj.GetIntValue());
            }
            else if (obj.IsDouble)
            {
                cefValue.SetDouble(obj.GetDoubleValue());
            }
            else if (obj.IsString)
            {
                cefValue.SetString(obj.GetStringValue());
            }
            else if (obj.IsDate)
            {
                // TODO time returned is UTC
                var date = obj.GetDateValue();
                var binary = CefValueSerialization.ToCefBinary(CefValueSerialization.BinaryMagicBytes.DateTime, BitConverter.GetBytes(date.ToBinary()));
                cefValue.SetBinary(binary);
            }
            else if (obj.IsArray)
            {
                var arrLength = obj.GetArrayLength();
                if (arrLength > 0)
                {
                    var keys = obj.GetKeys();
                    var cefList = CefListValue.Create();
                    for (var i = 0; i < arrLength; i++)
                    {
                        SerializeV8Object(obj.GetValue(keys[i]), new CefListWrapper(cefList, i), visitedObjects);
                    }

                    cefValue.SetList(cefList);
                }
                else
                {
                    cefValue.SetNull();
                }
            }
            else if (obj.IsFunction)
            {
                // TODO
                //    var context = CefV8Context.GetCurrentContext();
                //    var jsCallback = callbackRegistry.Register(context, obj);
                //    SetJsCallback(list, index, jsCallback);
            }
            else if (obj.IsObject)
            {
                var keys = obj.GetKeys();
                if (keys.Length > 0)
                {
                    var cefDictionary = CefDictionaryValue.Create();
                    foreach (var key in keys)
                    {
                        if (obj.HasValue(key))
                        {
                            SerializeV8Object(obj.GetValue(key), new CefDictionaryWrapper(cefDictionary, key), visitedObjects);
                        }
                    }
                    cefValue.SetDictionary(cefDictionary);
                }
            }
            else
            {
                cefValue.SetNull();
            }

            visitedObjects.Pop();
        }

        public static CefV8Value SerializeCefValue(CefValueWrapper cefValue)
        {
            switch (cefValue.GetValueType())
            {
                case CefValueType.Binary:
                    var nativeValue = CefValueSerialization.FromCefBinary(cefValue.GetBinary(), out var kind);
                    switch(kind)
                    {
                        case CefValueSerialization.BinaryMagicBytes.Binary:
                            // TODO not supported yet
                            throw new NotImplementedException("Cannot serialize a binary into a v8 object");

                        case CefValueSerialization.BinaryMagicBytes.DateTime:
                            return CefV8Value.CreateDate((DateTime)nativeValue);

                        default:
                            throw new NotImplementedException("Cannot serialize an unknown binary format into a v8 object");
                    }

                case CefValueType.Bool:
                    return CefV8Value.CreateBool(cefValue.GetBool());

                case CefValueType.Dictionary:
                    var dictionary = cefValue.GetDictionary();
                    var v8Dictionary = CefV8Value.CreateObject();
                    foreach (var key in dictionary.GetKeys())
                    {
                        v8Dictionary.SetValue(key, SerializeCefValue(new CefDictionaryWrapper(dictionary, key)));
                    }
                    return v8Dictionary;
                    
                case CefValueType.Double:
                    return CefV8Value.CreateDouble(cefValue.GetDouble());

                case CefValueType.List:
                    var list = cefValue.GetList();
                    var v8List = CefV8Value.CreateArray(list.Count);
                    for (var i = 0; i < list.Count; i++)
                    {
                        v8List.SetValue(i, SerializeCefValue(new CefListWrapper(list, i)));
                    }
                    return v8List;

                case CefValueType.Int:
                    return CefV8Value.CreateInt(cefValue.GetInt());

                case CefValueType.String:
                    return CefV8Value.CreateString(cefValue.GetString());

                case CefValueType.Null:
                    return CefV8Value.CreateNull();
            }

            return CefV8Value.CreateUndefined();
        }
    }
}
