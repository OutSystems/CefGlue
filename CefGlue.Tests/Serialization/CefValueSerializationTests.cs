﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Shared.Serialization;
using static Xilium.CefGlue.Common.Shared.Serialization.CefValueSerialization;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class SerializationTests
    {
        private enum DeserializationType
        {
            CefValue,
            Standard
        }

        private class Person
        {
            public Person() {}
            public Person(string name)
            {
                Name = name;
            }

            public string Name;
            public Person Parent;
            public Person Child;
        }

        private static JsonSerializerOptions _jsonDeserializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new StringJsonConverter()
            },
            IncludeFields = true,
            MaxDepth = 255,
            ReferenceHandler = ReferenceHandler.Preserve
        };

        private static object SerializeAndDeserialize<T>(T value, out CefValueType valueType, DeserializationType deserializationType = DeserializationType.CefValue)
        {
            var cefValue = new CefTestValue();
            Serialize(value, cefValue);
            var result =
                deserializationType == DeserializationType.CefValue
                ? DeserializeCefValue(cefValue)
                : DeserializeJsonString<T>(cefValue.GetString());
            valueType = cefValue.GetValueType();
            return result;
        }

        private static object DeserializeJsonString<T>(string jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString, _jsonDeserializerOptions);
        }

        private static void AssertSerialization<T>(T value, CefValueType valueType, DeserializationType deserializationType = DeserializationType.CefValue)
        {
            var obtainedValue = SerializeAndDeserialize<T>(value, out var obtainedValueType, deserializationType);
            Assert.AreEqual(value, obtainedValue);
            Assert.AreEqual(valueType, obtainedValueType);
        }

        [Test]
        public void HandlesNullObject()
        {
            AssertSerialization<object>(null, CefValueType.Null);
        }

        [Test]
        public void HandlesBooleans()
        {
            AssertSerialization(true, CefValueType.Bool);
            AssertSerialization(false, CefValueType.Bool);
        }

        [Test]
        public void HandlesSignedIntegers16()
        {
            AssertSerialization(Int16.MaxValue, CefValueType.Int);
        }

        [Test]
        public void HandlesSignedIntegers32()
        {
            AssertSerialization(Int32.MaxValue, CefValueType.Int);
        }

        [Test]
        public void HandlesSignedIntegers64()
        {
            AssertSerialization(Int64.MaxValue, CefValueType.Double);
        }

        [Test]
        public void HandlesUnsignedIntegers16()
        {
            AssertSerialization(UInt16.MinValue, CefValueType.Int);
        }

        [Test]
        public void HandlesUnsignedIntegers32()
        {
            AssertSerialization(UInt32.MinValue, CefValueType.Int);
        }

        [Test]
        public void HandlesUnsignedIntegers64()
        {
            AssertSerialization(UInt64.MinValue, CefValueType.Double);
        }

        [Test]
        public void HandlesBytes()
        {
            AssertSerialization((byte)12, CefValueType.Int);
        }

        [Test]
        public void HandlesStrings()
        {
            AssertSerialization("this is a string", CefValueType.String);
            AssertSerialization("", CefValueType.String);
        }

        [Test]
        public void HandlesStringsWithSpecialChars()
        {
            AssertSerialization("日本語組版処理の", CefValueType.String);
        }

        [Test]
        public void HandlesChars()
        {
            var value = SerializeAndDeserialize('c', out var valueType);
            Assert.AreEqual("c", value);
            Assert.AreEqual(CefValueType.String, valueType);
        }

        [Test]
        public void HandlesDoubles()
        {
            AssertSerialization(10.5d, CefValueType.Double);
        }

        [Test]
        public void HandlesFloats()
        {
            AssertSerialization(10.5f, CefValueType.Double);
        }

        [Test]
        public void HandlesDecimals()
        {
            AssertSerialization(10.5m, CefValueType.Double);
        }

        [Test]
        public void HandlesBinaries()
        {
            AssertSerialization(new byte[] { 0, 1, 2, 3 }, CefValueType.String);
            AssertSerialization(new byte[0], CefValueType.String);
        }

        [Test]
        public void HandlesDateTimes()
        {
            var date = new DateTime(2000, 1, 31, 15, 00, 10);
            AssertSerialization(date, CefValueType.String);
        }

        [Test]
        public void HandlesCyclicDictionaryReferences()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("first", dict);

            object obtainedValue = null;
            Assert.DoesNotThrow(() => obtainedValue = SerializeAndDeserialize(dict, out var _, DeserializationType.Standard));
            Assert.IsInstanceOf<Dictionary<string, object>>(obtainedValue);
            Assert.AreSame(obtainedValue,((Dictionary<string, object>)obtainedValue).First().Value);
        }

        [Test]
        public void HandlesCyclicListReferences()
        {
            var list = new List<object>();
            list.Add(list);

            object obtainedValue = null;
            Assert.DoesNotThrow(() => obtainedValue = SerializeAndDeserialize(list, out var _, DeserializationType.Standard));
            Assert.IsInstanceOf<List<object>>(obtainedValue);
            Assert.AreSame(obtainedValue,((List<object>)obtainedValue).First());
        }

        [Test]
        public void HandlesCyclicObjectReferences()
        {
            var parent = new Person("p");
            var child = new Person("c");
            child.Parent = parent;
            parent.Child = child;

            object obtainedValue = null;
            Assert.DoesNotThrow(() => obtainedValue = SerializeAndDeserialize(parent, out var _, DeserializationType.Standard));
            Assert.IsInstanceOf<Person>(obtainedValue);
            var obtainedPerson = (Person)obtainedValue;
            Assert.AreEqual(parent.Name, obtainedPerson.Name);
            Assert.NotNull(obtainedPerson.Child);
            Assert.AreEqual(parent.Child.Name, obtainedPerson.Child.Name);
            Assert.AreSame(obtainedPerson, obtainedPerson.Child.Parent);
        }

        [Test]
        public void HandlesLists()
        {
            var list = new List<string>() { "1", "2" };
            AssertSerialization(list, CefValueType.String, DeserializationType.Standard);
            AssertSerialization(new List<string>(), CefValueType.String, DeserializationType.Standard);
        }

        [Test]
        public void HandlesNestedLists()
        {
            var list = new List<List<string>>()
            {
                new List<string> { "1" , "2" },
                new List<string> { "3" , "4" }
            };
            AssertSerialization(list, CefValueType.String, DeserializationType.Standard);
        }

        [Test]
        public void HandlesListsOfObjects()
        {
            var list = new List<Dictionary<string, string>>()
            {
                new Dictionary<string, string>() {
                    { "first" , "1" },
                    { "second" , "2" },
                },
                new Dictionary<string, string>() {
                    { "third" , "3" },
                    { "fourth" , "4" },
                },
            };
            AssertSerialization(list, CefValueType.String, DeserializationType.Standard);
        }

        [Test]
        public void HandlesArrays()
        {
            var list = new string[] { "1", "2" };
            AssertSerialization(list, CefValueType.String);
            AssertSerialization(new string[0], CefValueType.String);
        }

        [Test]
        public void HandlesSerializationOfDeepListsWith250Levels()
        {
            var list = new List<object>();
            var child = new List<object>();
            list.Add(child);

            for (var i = 0; i < 250; i++)
            {
                var nestedChild = new List<object>();
                child.Add(nestedChild);
                child = nestedChild;
            }

            var cefValue = new CefTestValue();
            Assert.DoesNotThrow(() => Serialize(list, cefValue));
        }

        [Test]
        public void HandlesDeserializationOfDeepListsWith250LevelsWithoutReferences()
        {
            var list = new List<object>();
            var child = new List<object>();
            list.Add(child);

            for (var i = 0; i < 250; i++)
            {
                var nestedChild = new List<object>();
                child.Add(nestedChild);
                child = nestedChild;
            }

            var cefValue = new CefTestValue();
            // use a default serializer, without references handling
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                IncludeFields = true,
                MaxDepth = int.MaxValue,
            };
            var json = JsonSerializer.Serialize(list, jsonSerializerOptions);
            cefValue.SetString(json);
            object obtainedValue = null;

            Assert.DoesNotThrow(() => obtainedValue = DeserializeCefValue(cefValue));
            var valueType = cefValue.GetValueType();
            Assert.IsTrue(valueType == CefValueType.String);
            Assert.IsInstanceOf<object[]>(obtainedValue);
        }

        [Test]
        public void HandlesListWithObjectReferences()
        {
            var list = new List<object>();
            var childList = new List<object>();
            list.Add(childList);
            list.Add(childList);

            var cefValue = new CefTestValue();
            Assert.DoesNotThrow(() => Serialize(list, cefValue));
            Assert.IsTrue(cefValue.GetString().TrimEnd('}', ']').EndsWith("\"$ref\":\"2\""), "The last element in the list should be a reference to the first child");
        }

        [Test]
        public void HandlesSimpleDictionaries()
        {
            var dict = new Dictionary<string, object>()
            {
                { "first", 1 },
                { "second", "string" },
                { "third", true },
                { "fourth", new DateTime() },
                { "fifth", new byte[] { 0 , 1, 2 } },
                { "sixth", null },
                { "seventh", 7.0 }
            };

            var cefValue = new CefTestValue();
            Serialize(dict, cefValue);
            // strip the "$id:1" from the json string so it can be passed do the deserializer, who doesn't handle references
            cefValue.SetString(cefValue.GetString().Replace("\"$id\":\"1\",", ""));
            var obtainedValue = DeserializeCefValue(cefValue);
            Assert.AreEqual(dict, obtainedValue);
            Assert.AreEqual(cefValue.GetValueType(), CefValueType.String);
        }

        [Test]
        public void HandlesNestedDictionaries()
        {
            var dict = new Dictionary<string, Dictionary<string, double>>()
            {
                { "first", new Dictionary<string, double>() {
                    { "first_first", 1d },
                    { "first_second", 2d },
                    { "first_third", 3d },
                }},
                { "second", new Dictionary<string, double>() {
                    { "second_first", 4d },
                    { "second_second", 5d },
                    { "second_third", 6d },
                }},
                { "third", new Dictionary<string, double>() {
                    { "third_first", 7d },
                    { "third_second", 8d },
                    { "third_third", 9d },
                }}
            };
            AssertSerialization(dict, CefValueType.String, DeserializationType.Standard);
        }

        [Test]
        public void HandlesObjects()
        {
            var obj = new ParentObj()
            {
                stringField = "parent",
                childObj = new ChildObj()
                {
                    stringField = "child",
                    intField = 1,
                    boolField = true,
                    dateField = DateTime.Now,
                    doubleField = 5.5,
                    binaryField = new byte[] {0, 1, 2}
                }
            };

            var obtainedValue = (Dictionary<string, object>) SerializeAndDeserialize(obj, out var valueType);
            Assert.AreEqual(CefValueType.String, valueType);
            Assert.AreEqual(obj.stringField, obtainedValue[nameof(ParentObj.stringField)]);
            var child = obj.childObj;
            var obtainedChild = (Dictionary<string, object>) obtainedValue[nameof(ParentObj.childObj)];
            Assert.AreEqual(child.binaryField, obtainedChild[nameof(ChildObj.binaryField)]);
            Assert.AreEqual(child.boolField, obtainedChild[nameof(ChildObj.boolField)]);
            Assert.AreEqual(child.dateField, obtainedChild[nameof(ChildObj.dateField)]);
            Assert.AreEqual(child.doubleField, obtainedChild[nameof(ChildObj.doubleField)]);
            Assert.AreEqual(child.intField, obtainedChild[nameof(ChildObj.intField)]);
            Assert.AreEqual(child.stringField, obtainedChild[nameof(ChildObj.stringField)]);
        }
    }
}
