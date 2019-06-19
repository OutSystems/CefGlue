using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Xilium.CefGlue.Common
{
    internal static class V8Serialization
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
                var binaryValue = CefBinaryValue.Create(BitConverter.GetBytes(date.ToBinary()));
                container.SetBinary(binaryValue);
            }
            else if (obj.IsArray)
            {
                var arrLength = obj.GetArrayLength();
                if (arrLength > 0)
                {
                    var keys =  obj.GetKeys();
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

        public static object DeserializeV8Object(CefValue value)
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
                    foreach(var key in keys)
                    {
                        dictionary[key] = DeserializeV8Object(v8Dictionary.GetValue(key));
                    }
                    return dictionary;

                case CefValueType.Double:
                    return value.GetDouble();

                case CefValueType.List:
                    return DeserializeV8List<object>(value.GetList());

                case CefValueType.Int:
                    return value.GetInt();

                case CefValueType.String:
                    return value.GetString();

                case CefValueType.Null:
                    return null;
            }

            return null;
        }

        public static ListElementType[] DeserializeV8List<ListElementType>(CefListValue v8List)
        {
            var array = new ListElementType[v8List.Count];
            for (var i = 0; i < v8List.Count; i++)
            {
                array[i] = (ListElementType) DeserializeV8Object(v8List.GetValue(i));
            }
            return array;
        }
    }
}
