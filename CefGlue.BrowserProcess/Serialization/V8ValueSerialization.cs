using System;
using System.Linq;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.BrowserProcess.Serialization
{
    internal static class V8ValueSerialization
    {
        /// <summary>
        /// Converts a V8Value to a CefValue (used when sending values from the browser process to the main process)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cefValue"></param>
        public static void SerializeV8ObjectToCefValue(CefV8Value obj, CefValueWrapper cefValue)
        {
            SerializeV8ObjectToCefValue(obj, cefValue, new ReferencesResolver<CefV8Value>(CefV8ValueEqualityComparer.Instance));
        }

        private static void SerializeV8ObjectToCefValue(CefV8Value obj, CefValueWrapper cefValue, IReferencesResolver<CefV8Value> referencesResolver)
        {
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
                // default to "", because cef converts "" to null, and when null it will fall on the Null case
                CefValueSerialization.Serialize(obj.GetStringValue() ?? "", cefValue);
            }
            else if (obj.IsDate)
            {
                CefValueSerialization.Serialize(obj.GetDateValue(), cefValue);
            }
            else if (obj.IsArray)
            {
                var arrLength = obj.GetArrayLength();
                if (arrLength > 0)
                {
                    using (var cefDictionary = CefDictionaryValue.Create())
                    {
                        if (!TryHandleExistingObjectReference(obj, cefDictionary, referencesResolver))
                        {
                            using (var cefList = CefListValue.Create())
                            {
                                HandleNewObjectReference(obj, cefDictionary, referencesResolver);

                                var keys = obj.GetKeys();

                                for (var i = 0; i < arrLength; i++)
                                {
                                    SerializeV8ObjectToCefValue(obj.GetValue(keys[i]), new CefListWrapper(cefList, i), referencesResolver);
                                }

                                cefDictionary.SetList(JsonAttributes.Values, cefList);
                            }
                        }

                        cefValue.SetDictionary(cefDictionary);
                    }
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
                    using (var cefDictionary = CefDictionaryValue.Create())
                    {
                        if (!TryHandleExistingObjectReference(obj, cefDictionary, referencesResolver))
                        {
                            HandleNewObjectReference(obj, cefDictionary, referencesResolver);
                            
                            foreach (var key in keys.Where(k => obj.HasValue(k)))
                            {
                                SerializeV8ObjectToCefValue(obj.GetValue(key), new CefDictionaryWrapper(cefDictionary, key), referencesResolver);
                            }
                        }

                        cefValue.SetDictionary(cefDictionary);
                    }
                }
            }
            else
            {
                cefValue.SetNull();
            }
        }

        /// <summary>
        /// Converts a CefValue to V8Value (used when sending values to the JS context)
        /// </summary>
        /// <param name="cefValue"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static CefV8Value SerializeCefValue(CefValueWrapper cefValue)
        {
            switch (cefValue.GetValueType())
            {
                case CefValueType.Binary:
                    // binaries are serialized as base64
                    throw new InvalidOperationException("Cannot serialize a Binary into a v8 object");

                case CefValueType.Bool:
                    return CefV8Value.CreateBool(cefValue.GetBool());

                case CefValueType.Dictionary:
                    // dictionaries are serialized as json
                    throw new InvalidOperationException("Cannot serialize a Dictionary into a v8 object");

                case CefValueType.Double:
                    return CefV8Value.CreateDouble(cefValue.GetDouble());

                case CefValueType.List:
                    // lists are serialized as json
                    throw new InvalidOperationException("Cannot serialize a List into a v8 object");

                case CefValueType.Int:
                    return CefV8Value.CreateInt(cefValue.GetInt());

                case CefValueType.String:
                    return CefV8Value.CreateString(cefValue.GetString());

                case CefValueType.Null:
                    return CefV8Value.CreateNull();
            }

            return CefV8Value.CreateUndefined();
        }

        private static bool TryHandleExistingObjectReference(CefV8Value obj, CefDictionaryValue cefDictionary, IReferencesResolver<CefV8Value> referencesResolver)
        {
            if (!referencesResolver.TryGetReferenceId(obj, out var refId))
            {
                return false;
            }

            cefDictionary.SetString(JsonAttributes.Ref, refId);
            return true;
        }

        private static void HandleNewObjectReference(CefV8Value obj, CefDictionaryValue cefDictionary, IReferencesResolver<CefV8Value> referencesResolver)
        {
            var refId = referencesResolver.AddReference(obj);
            cefDictionary.SetString(JsonAttributes.Id, refId);
        }
    }
}
