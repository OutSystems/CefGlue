using System;
using System.Collections.Generic;
using System.Linq;
using Xilium.CefGlue.Common.Serialization;

namespace Xilium.CefGlue.BrowserProcess.Serialization
{
    internal static class V8ValueSerialization
    {
        private abstract class BaseContainer
        {
            public abstract void SetNull();
            public abstract void SetBool(bool value);
            public abstract void SetInt(int value);
            public abstract void SetDouble(double value);
            public abstract void SetString(string value);
            public abstract void SetBinary(CefBinaryValue value);
            public abstract void SetList(CefListValue value);
            public abstract void SetDictionary(CefDictionaryValue value);
        }

        private abstract class BaseContainer<TIndex, TCefContainerUnderlyingType> : BaseContainer
        {
            protected readonly TIndex _index;
            protected readonly TCefContainerUnderlyingType _container;

            public BaseContainer(TCefContainerUnderlyingType container, TIndex index)
            {
                _index = index;
                _container = container;
            }
        }

        private class ListContainer : BaseContainer<int, CefListValue>
        {
            public ListContainer(CefListValue container, int index) : base(container, index)
            {
            }

            public override void SetNull()
            {
                _container.SetNull(_index);
            }

            public override void SetBool(bool value)
            {
                _container.SetBool(_index, value);
            }

            public override void SetInt(int value)
            {
                _container.SetInt(_index, value);
            }

            public override void SetDouble(double value)
            {
                _container.SetDouble(_index, value);
            }

            public override void SetString(string value)
            {
                _container.SetString(_index, value);
            }

            public override void SetBinary(CefBinaryValue value)
            {
                _container.SetBinary(_index, value);
            }

            public override void SetList(CefListValue value)
            {
                _container.SetList(_index, value);
            }

            public override void SetDictionary(CefDictionaryValue value)
            {
                _container.SetDictionary(_index, value);
            }
        }

        private class DictionaryContainer : BaseContainer<string, CefDictionaryValue>
        {
            public DictionaryContainer(CefDictionaryValue container, string index) : base(container, index)
            {
            }

            public override void SetNull()
            {
                _container.SetNull(_index);
            }

            public override void SetBool(bool value)
            {
                _container.SetBool(_index, value);
            }

            public override void SetInt(int value)
            {
                _container.SetInt(_index, value);
            }

            public override void SetDouble(double value)
            {
                _container.SetDouble(_index, value);
            }

            public override void SetString(string value)
            {
                _container.SetString(_index, value);
            }

            public override void SetBinary(CefBinaryValue value)
            {
                _container.SetBinary(_index, value);
            }

            public override void SetList(CefListValue value)
            {
                _container.SetList(_index, value);
            }

            public override void SetDictionary(CefDictionaryValue value)
            {
                _container.SetDictionary(_index, value);
            }
        }

        public static void SerializeV8Object(CefV8Value obj, CefListValue container, int containerIndex)
        {
            SerializeV8Object(obj, new ListContainer(container, containerIndex), new Stack<CefV8Value>());
        }

        public static CefListValue SerializeV8Object(CefV8Value[] objs)
        {
            var list = CefListValue.Create();
            for (var i = 0; i < objs.Length; i++)
            {
                SerializeV8Object(objs[i], new ListContainer(list, i), new Stack<CefV8Value>());
            }
            return list;
        }

        private static void SerializeV8Object(CefV8Value obj, BaseContainer container, Stack<CefV8Value> visitedObjects)
        {
            if (visitedObjects.Any(o => o.IsSame(obj)))
            {
                throw new InvalidOperationException("Cycle found in result");
            }

            visitedObjects.Push(obj);

            if (obj.IsNull || obj.IsUndefined)
            {
                container.SetNull();
            }
            else if (obj.IsBool)
            {
                container.SetBool(obj.GetBoolValue());
            }
            else if (obj.IsInt)
            {
                container.SetInt(obj.GetIntValue());
            }
            else if (obj.IsDouble)
            {
                container.SetDouble(obj.GetDoubleValue());
            }
            else if (obj.IsString)
            {
                container.SetString(obj.GetStringValue());
            }
            else if (obj.IsDate)
            {
                // TODO time returned is UTC
                var date = obj.GetDateValue();
                var binary = CefValueSerialization.ToCefBinary(CefValueSerialization.BinaryMagicBytes.DateTime, BitConverter.GetBytes(date.ToBinary()));
                container.SetBinary(binary);
            }
            else if (obj.IsArray)
            {
                var arrLength = obj.GetArrayLength();
                if (arrLength > 0)
                {
                    var keys = obj.GetKeys();
                    var array = CefListValue.Create();
                    for (var i = 0; i < arrLength; i++)
                    {
                        SerializeV8Object(obj.GetValue(keys[i]), new ListContainer(array, i), visitedObjects);
                    }

                    container.SetList(array);
                }
                else
                {
                    container.SetNull();
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
                    var dictionary = CefDictionaryValue.Create();
                    foreach (var key in keys)
                    {
                        if (obj.HasValue(key) && !key.StartsWith("__"))
                        {
                            SerializeV8Object(obj.GetValue(key), new DictionaryContainer(dictionary, key), visitedObjects);
                        }
                    }
                    container.SetDictionary(dictionary);
                }
            }
            else
            {
                container.SetNull();
            }

            visitedObjects.Pop();
        }

        public static CefV8Value SerializeCefValue(CefValue value)
        {
            switch (value.GetValueType())
            {
                case CefValueType.Binary:
                    var nativeValue = CefValueSerialization.FromCefBinary(value.GetBinary(), out var kind);
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
                    return CefV8Value.CreateBool(value.GetBool());

                case CefValueType.Dictionary:
                    var dictionary = value.GetDictionary();
                    var v8Dictionary = CefV8Value.CreateObject();
                    foreach (var key in dictionary.GetKeys())
                    {
                        v8Dictionary.SetValue(key, SerializeCefValue(dictionary.GetValue(key)));
                    }
                    return v8Dictionary;

                case CefValueType.Double:
                    return CefV8Value.CreateDouble(value.GetDouble());

                case CefValueType.List:
                    var list = value.GetList();
                    var v8List = CefV8Value.CreateArray(list.Count);
                    for (var i = 0; i < list.Count; i++)
                    {
                        v8List.SetValue(i, SerializeCefValue(list.GetValue(i)));
                    }
                    return v8List;

                case CefValueType.Int:
                    return CefV8Value.CreateInt(value.GetInt());

                case CefValueType.String:
                    return CefV8Value.CreateString(value.GetString());

                case CefValueType.Null:
                    return CefV8Value.CreateNull();
            }

            return CefV8Value.CreateUndefined();
        }
    }
}
