using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.BrowserProcess.Serialization
{
    internal static class V8ValueSerialization
    {
        private const string JsonAttributeIdPropName = "$id";
        private const string JsonAttributeRefPropName = "$ref";

        /// <summary>
        /// Converts a V8Value to a CefValue (used when sending values from the browser process to the main process)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cefValue"></param>
        public static void SerializeV8ObjectToCefValue(CefV8Value obj, CefValueWrapper cefValue)
        {
            SerializeV8ObjectToCefValue(obj, cefValue, new CefReferencesResolver<CefV8Value>(CefV8ValueEqualityComparer.Instance));
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
                    var keys = obj.GetKeys();

                    using (var cefList = CefListValue.Create())
                    {
                        for (var i = 0; i < arrLength; i++)
                        {
                            SerializeV8ObjectToCefValue(obj.GetValue(keys[i]), new CefListWrapper(cefList, i), referencesResolver);
                        }

                        cefValue.SetList(cefList);
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
                        if (referencesResolver.TryGetReferenceId(obj, out var refId))
                        {
                            var dictWrapper = new CefDictionaryWrapper(cefDictionary, JsonAttributeRefPropName);
                            dictWrapper.SetString(refId);
                        }
                        else
                        {
                            refId = referencesResolver.ReferencesCount.ToString();
                            referencesResolver.AddReference(refId, obj);
                            var dictWrapper = new CefDictionaryWrapper(cefDictionary, JsonAttributeIdPropName);
                            dictWrapper.SetString(refId);

                            foreach (var key in keys)
                            {
                                if (obj.HasValue(key))
                                {
                                    SerializeV8ObjectToCefValue(obj.GetValue(key), new CefDictionaryWrapper(cefDictionary, key), referencesResolver);
                                }
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
    }
}
