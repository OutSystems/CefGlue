using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Xilium.CefGlue;
using static Xilium.CefGlue.Common.Shared.Serialization.CefValueSerialization;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class SerializationTests
    {
        private static object SerializeAndDeserialize(object value, out CefValueType valueType)
        {
            var cefValue = new CefTestValue();
            Serialize(value, cefValue);
            var result = DeserializeCefValue(cefValue);
            valueType = cefValue.GetValueType();
            return result;
        }

        private static void AssertSerialization(object value, CefValueType valueType)
        {
            var obtainedValue = SerializeAndDeserialize(value, out var obtainedValueType);
            Assert.AreEqual(value, obtainedValue);
            Assert.AreEqual(valueType, obtainedValueType);
        }

        [Test]
        public void HandlesNullObject()
        {
            AssertSerialization(null, CefValueType.Null);
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
            Assert.DoesNotThrow(() => obtainedValue = SerializeAndDeserialize(dict, out var _));
            Assert.IsInstanceOf<Dictionary<string, object>>(obtainedValue);
            Assert.AreSame(obtainedValue,((Dictionary<string, object>)obtainedValue).First().Value);
        }

        [Test]
        public void HandlesCyclicListReferences()
        {
            var list = new List<object>();
            list.Add(list);

            object obtainedValue = null;
            Assert.DoesNotThrow(() => obtainedValue = SerializeAndDeserialize(list, out var _));
            Assert.IsInstanceOf<List<object>>(obtainedValue);
            Assert.AreSame(obtainedValue,((List<object>)obtainedValue).First());
        }

        private class Person
        {
            public Person Parent;
            public Person Child;
        }

        [Test]
        public void HandlesCyclicObjectReferences()
        {
            var parent = new Person();
            var child = new Person();
            child.Parent = parent;
            parent.Child = child;

            object obtainedValue = null;
            Assert.DoesNotThrow(() => obtainedValue = SerializeAndDeserialize(parent, out var _));
            // the Cef"Deserializer" returns a Dictionary<string, object> for Objects
            Assert.IsInstanceOf<Dictionary<string, object>>(obtainedValue);
            Assert.AreEqual(2, ((Dictionary<string, object>)obtainedValue).Count);
            Assert.AreEqual("Parent", ((Dictionary<string, object>)obtainedValue).Keys.First());
            Assert.AreEqual("Child", ((Dictionary<string, object>)obtainedValue).Keys.Last());
        }

        [Test]
        public void HandlesLists()
        {
            var list = new List<string>() { "1", "2" };
            AssertSerialization(list, CefValueType.String);
            AssertSerialization(new List<string>(), CefValueType.String);
        }

        [Test]
        public void HandlesNestedLists()
        {
            var list = new List<List<string>>()
            {
                new List<string> { "1" , "2" },
                new List<string> { "3" , "4" }
            };
            AssertSerialization(list, CefValueType.String);
        }

        [Test]
        public void HandlesListsOfObjects()
        {
            var list = new List<object>()
            {
                new Dictionary<string, object>() {
                    { "first" , "1" },
                    { "second" , "2" },
                },
                new Dictionary<string, object>() {
                    { "third" , "3" },
                    { "fourth" , "4" },
                },
            };
            AssertSerialization(list, CefValueType.String);
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
        public void HandlesDeserializationOfDeepListsWith248LevelsWithoutReferences()
        {
            var list = new List<object>();
            var child = new List<object>();
            list.Add(child);

            // 248 is the maximum supported deepness for Cef Deserialization process
            // more than that and it will hit an exception
            for (var i = 0; i < 248; i++)
            {
                var nestedChild = new List<object>();
                child.Add(nestedChild);
                child = nestedChild;
            }

            var cefValue = new CefTestValue();
            // use a default serializer, without references handling
            var jsonSerializerOptions = new System.Text.Json.JsonSerializerOptions()
            {
                IncludeFields = true,
                MaxDepth = int.MaxValue,
            };
            var json = System.Text.Json.JsonSerializer.Serialize(list, jsonSerializerOptions);
            cefValue.SetString(json);
            object obtainedValue = null;

            Assert.DoesNotThrow(() => obtainedValue = DeserializeCefValue(cefValue));
            var valueType = cefValue.GetValueType();
            Assert.IsTrue(valueType == CefValueType.String);
            Assert.IsInstanceOf<object[]>(obtainedValue);
        }

        [Test]
        public void HandlesReferenceObjects()
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

            AssertSerialization(dict, CefValueType.String);
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
            AssertSerialization(dict, CefValueType.String);
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
