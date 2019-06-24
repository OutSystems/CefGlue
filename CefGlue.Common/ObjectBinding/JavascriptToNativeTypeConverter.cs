using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal static class JavascriptToNativeTypeConverter
    {
        private readonly static MethodInfo ToArrayMethodInfo = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);

        /// <summary>
        /// Converts an object from javascript into 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="expectedType"></param>
        /// <returns></returns>
        public static object ConvertToNative(object obj, Type expectedType)
        {
            if (obj == null)
            {
                return null;
            }

            var objType = obj.GetType();

            // expectedType can be assigned directly from objectType
            if (expectedType.IsAssignableFrom(objType))
            {
                return obj;
            }

            // expectedType is an enum and object too
            if (expectedType.IsEnum && expectedType.IsEnumDefined(obj))
            {
                return Enum.ToObject(expectedType, obj);
            }

            // object can be converted to expectedType (eg: double to int)
            var typeConverter = TypeDescriptor.GetConverter(objType);
            if (typeConverter.CanConvertTo(expectedType))
            {
                return typeConverter.ConvertTo(obj, expectedType);
            }
            
            if (expectedType.IsCollection() || expectedType.IsArray() || expectedType.IsEnumerable())
            {
                return ConvertToNativeList(obj, expectedType);
            }

            return ConvertToNativeObject(obj, expectedType);
        }

        private static bool IsCollection(this Type source)
        {
            var collectionType = typeof(ICollection<>);
            return source.GetTypeInfo().IsGenericType && source.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == collectionType);
        }

        private static bool IsArray(this Type source)
        {
            return source.GetTypeInfo().BaseType == typeof(Array);
        }

        private static bool IsEnumerable(this Type source)
        {
            return source.GetTypeInfo().IsGenericType && source.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static object ConvertToNativeList(object obj, Type expectedType)
        {
            if (!(obj is ICollection collection))
            {
                return null;
            }

            Type genericType = null;

            // Make sure it has a generic type
            if (expectedType.GetTypeInfo().IsGenericType)
            {
                genericType = expectedType.GetGenericArguments().FirstOrDefault();
            }
            else
            {
                var ienumerable = expectedType.GetInterfaces().Where(i => i.GetTypeInfo().IsGenericType).FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
                genericType = ienumerable?.GetGenericArguments().FirstOrDefault();
            }

            if (genericType == null)
            {
                // If we don't have a generic type then just use object
                genericType = typeof(object);
            }

            var listType = typeof(List<>).MakeGenericType(genericType);
            var nativeList = (IList)Activator.CreateInstance(listType);

            foreach (var item in collection)
            {
                // if the value is null then we'll add null to the collection,
                if (item == null)
                {
                    // for value types like int we'll create the default value and assign that as we cannot assign null
                    nativeList.Add(genericType.IsValueType ? Activator.CreateInstance(genericType) : null);
                }
                else
                {
                    var itemType = item.GetType();
                    // if the collection item is a list or dictionary then we'll attempt to convert it
                    if (typeof(IDictionary<string, object>).IsAssignableFrom(itemType) ||
                        typeof(IList<object>).IsAssignableFrom(itemType))
                    {
                        var subValue = ConvertToNative(item, genericType);
                        nativeList.Add(subValue);
                    }
                    else
                    {
                        nativeList.Add(item);
                    }
                }
            }

            if (expectedType.IsArray())
            {
                // TODO is there any other way to do this?
                var genericToArrayMethod = ToArrayMethodInfo.MakeGenericMethod(new[] { genericType });
                return genericToArrayMethod.Invoke(null, new[] { nativeList });
            }

            return nativeList;
        }

        private static object ConvertToNativeObject(object obj, Type expectedType)
        {
            var nativeObject = Activator.CreateInstance(expectedType, true);

            // if the object type is a dictionary, then attempt to convert all the members
            if (obj is IDictionary<string, object> dictionary)
            {
                var members = CollectMembers(expectedType);
                foreach (var property in members)
                {
                    if (dictionary.TryGetValue(property.Name, out var value))
                    {
                        var convertedValue = ConvertToNative(value, property.Type);
                        property.SetValue(nativeObject, convertedValue);
                    }
                }
            }

            return nativeObject;
        }

        private static IEnumerable<MemberInfo> CollectMembers(Type type)
        {
            var eligibleMembers = BindingFlags.Public | BindingFlags.Instance;

            var properties = type
                .GetProperties(eligibleMembers)
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => !p.GetIndexParameters().Any())
                .Select(p => new MemberInfo(p.Name, p.PropertyType, (obj, value) => p.SetValue(obj, value)));

            var fields = type
                .GetFields(eligibleMembers)
                .Where(f => !f.IsInitOnly)
                .Select(f => new MemberInfo(f.Name, f.FieldType, (obj, value) => f.SetValue(obj, value)));

            return properties.Concat(fields);
        }
    }
}
