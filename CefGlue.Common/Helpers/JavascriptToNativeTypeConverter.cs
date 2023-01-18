using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Xilium.CefGlue.Common.Helpers
{
    internal static class JavascriptToNativeTypeConverter
    {
        private delegate IList ListContructor(int length, out Type genericType);

        private static readonly DateTime _win32Epoch = new DateTime(1601, 1, 1);
        private static readonly IDictionary<Type, MemberInfo[]> _typeMembersCache = new Dictionary<Type, MemberInfo[]>();
        private static readonly IDictionary<Type, ListContructor> _typeListConstructorCache = new Dictionary<Type, ListContructor>();

        /// <summary>
        /// Converts an object from javascript into a .Net type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertToNative<T>(object obj)
        {
            return (T) ConvertToNative(obj, typeof(T));
        }

        /// <summary>
        /// Converts an object from javascript into a .Net type
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="expectedType">Expected object type</param>
        /// <returns></returns>
        public static object ConvertToNative(object obj, Type expectedType)
        {
            if (obj == null)
            {
                if (expectedType.IsValueType)
                {
                    // return type default value
                    return Activator.CreateInstance(expectedType);
                }
                return null;
            }

            if (expectedType == typeof(double) && obj is int)
            {
                // javascript numbers sometimes can lose the decimal part becoming integers, prevent that
                return Convert.ToDouble(obj);
            }

            if (expectedType == typeof(DateTime)
                && obj is Dictionary<string,object> { Count: 1 } objValue
                && objValue.TryGetValue("Ticks", out var dateTicksInMicroseconds)) {
                var milliseconds = Convert.ToDouble(dateTicksInMicroseconds) / 1000;
                return _win32Epoch.AddMilliseconds(milliseconds);
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

            var nativeList = GetListConstructor(expectedType)(collection.Count, out var listItemType);

            var i = 0;

            foreach (var item in collection)
            {
                var convertedItem = item;
                // if the value is null then we'll add null to the collection,
                if (item == null)
                {
                    // for value types like int we'll create the default value and assign that as we cannot assign null
                    convertedItem = listItemType.IsValueType ? Activator.CreateInstance(listItemType) : null;
                }
                else
                {
                    var itemType = item.GetType();
                    // if the collection item is a list or dictionary then we'll attempt to convert it
                    if (typeof(IDictionary<string, object>).IsAssignableFrom(itemType) ||
                        typeof(IList<object>).IsAssignableFrom(itemType))
                    {
                        convertedItem = ConvertToNative(item, listItemType);
                    }
                }

                nativeList[i++] = convertedItem;
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

        private static ListContructor GetListConstructor(Type expectedType)
        {
            if (!_typeListConstructorCache.TryGetValue(expectedType, out var listContructor))
            {
                lock (_typeListConstructorCache)
                {
                    if (!_typeListConstructorCache.TryGetValue(expectedType, out listContructor))
                    {
                        listContructor = InnerGetListConstructor(expectedType);
                        _typeListConstructorCache[expectedType] = listContructor;
                    }
                }
            }
            return listContructor;
        }

        private static ListContructor InnerGetListConstructor(Type expectedType)
        {
            Type listType = null;
            Type listItemType = null;

            // make sure it has a generic type
            if (expectedType.GetTypeInfo().IsGenericType)
            {
                listType = expectedType;
            }
            else
            {
                listType = expectedType.GetInterfaces().Where(itf => itf.GetTypeInfo().IsGenericType).FirstOrDefault(itf => itf.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            }

            // if we don't have a generic type then just use object
            listItemType = listType?.GetGenericArguments().FirstOrDefault() ?? typeof(object);

            if (expectedType.IsArray())
            {
                return (int length, out Type _listItemType) =>
                {
                    _listItemType = listItemType;
                    return Array.CreateInstance(listItemType, length);
                };
            }

            var listGenericType = typeof(List<>).MakeGenericType(listItemType);
            return (int length, out Type _listItemType) =>
            {
                _listItemType = listItemType;
                return (IList)Activator.CreateInstance(listGenericType, new object[] { length });
            };
        }

        private static IEnumerable<MemberInfo> CollectMembers(Type type)
        {
            if (!_typeMembersCache.TryGetValue(type, out var members))
            {
                lock (_typeMembersCache)
                {
                    if (!_typeMembersCache.TryGetValue(type, out members))
                    {
                        members = InnerCollectMembers(type).ToArray();
                        _typeMembersCache[type] = members;
                    }
                }
            }
            return members;
        }

        private static IEnumerable<MemberInfo> InnerCollectMembers(Type type)
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
