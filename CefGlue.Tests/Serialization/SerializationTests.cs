using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using Xilium.CefGlue.Common.Shared.Serialization;
using static Xilium.CefGlue.Common.Shared.Serialization.Serializer;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class SerializationTests
    {
        private static ObjectType SerializeAndDeserialize<ObjectType>(ObjectType value)
        {
            var json = Serialize(value);
            var result = Deserializer.Deserialize<ObjectType>(json);
            return result;
        }

        private static ObjectType AssertSerialization<ObjectType>(ObjectType value, bool assertEquality = true)
        {
            var obtainedValue = SerializeAndDeserialize(value);
            Assert.AreSame(value?.GetType(), obtainedValue?.GetType());
            if (assertEquality)
            {
                Assert.AreEqual(value, obtainedValue);
            }
            return obtainedValue;
        }

        [Test]
        public void HandlesNullObject()
        {
            AssertSerialization((object)null);
        }

        [Test]
        public void HandlesPlainObject()
        {
            var obtainedValue = AssertSerialization(new object(), assertEquality: false);
            Assert.NotNull(obtainedValue);
        }

        [Test]
        public void HandlesBooleans()
        {
            AssertSerialization(true);
            AssertSerialization(false);
        }

        [Test]
        public void HandlesSignedIntegers16()
        {
            AssertSerialization(Int16.MaxValue);
        }

        [Test]
        public void HandlesSignedIntegers32()
        {
            AssertSerialization(Int32.MaxValue);
        }

        [Test]
        public void HandlesSignedIntegers64()
        {
            AssertSerialization(Int64.MaxValue);
        }

        [Test]
        public void HandlesUnsignedIntegers16()
        {
            AssertSerialization(UInt16.MinValue);
        }

        [Test]
        public void HandlesUnsignedIntegers32()
        {
            AssertSerialization(UInt32.MinValue);
        }

        [Test]
        public void HandlesUnsignedIntegers64()
        {
            AssertSerialization(UInt64.MinValue);
        }

        [Test]
        public void HandlesBytes()
        {
            AssertSerialization((byte)12);
        }

        [Test]
        public void HandlesStrings()
        {
            AssertSerialization("this is a string");
            AssertSerialization("");
        }

        [Test]
        public void HandlesStringsWithSpecialChars()
        {
            AssertSerialization("日本語組版処理の");
        }

        [Test]
        public void HandlesChars()
        {
            AssertSerialization('c');
        }

        [Test]
        public void HandlesDoubles()
        {
            AssertSerialization(10.5d);
        }

        [Test]
        public void HandlesFloats()
        {
            AssertSerialization(10.5f);
        }

        [Test]
        public void HandlesDecimals()
        {
            AssertSerialization(10.5m);
        }

        [Test]
        public void HandlesBinaries()
        {
            AssertSerialization(new byte[] { 0, 1, 2, 3 });
            AssertSerialization(new byte[0]);
        }

        [Test]
        public void HandlesDateTimes()
        {
            var date = new DateTime(2000, 1, 31, 15, 00, 10);
            AssertSerialization(date);
        }

        [Test]
        public void HandlesCyclicDictionaryReferences()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("first", dict);

            var obtainedValue = AssertSerialization(dict, assertEquality: false);
            Assert.AreSame(obtainedValue, obtainedValue.First().Value);
        }

        [Test]
        public void HandlesCyclicListReferences()
        {
            var list = new List<object>();
            list.Add(list);

            var obtainedValue = AssertSerialization(list, assertEquality: false);
            Assert.AreSame(obtainedValue, obtainedValue.First());
        }

        [Test]
        public void HandlesCyclicObjectReferences()
        {
            var parent = new Person("p");
            var child = new Person("c");
            child.Parent = parent;
            parent.Child = child;

            var obtainedValue = AssertSerialization(parent, assertEquality: false);
            Assert.AreSame(obtainedValue, obtainedValue.Child.Parent, "Child.Parent instance should point to the obtained dictionary instance.");
        }

        [Test]
        public void HandlesLists()
        {
            var list = new List<string>() { "1", "2" };
            AssertSerialization(list);
            AssertSerialization(new List<string>());
        }

        [Test]
        public void HandlesNestedLists()
        {
            var list = new List<List<string>>()
            {
                new List<string> { "1" , "2" },
                new List<string> { "3" , "4" }
            };
            AssertSerialization(list);
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
            AssertSerialization(list);
        }

        [Test]
        public void HandlesDictionaryOfPrimitiveValueTypeCollection()
        {
            var dict = new Dictionary<string, int>() {
                    { "first" , 1 },
                    { "second" , 2 },
                    { "third" , 3 },
                };
            AssertSerialization(dict);
        }

        [Test]
        public void HandlesArrays()
        {
            var list = new string[] { "1", "2" };
            AssertSerialization(list);
            AssertSerialization(new string[0]);
        }

        [Test]
        public void HandlesArraysOfStructs()
        {
            var list = new StructObject[] { new StructObject("first", 1), new StructObject("second", 2) };
            AssertSerialization(list);
        }

        [Test]
        public void HandlesObjectsArray()
        {
            var list = new object[]
            {
                "1",
                null,
                new string[] {"a", "b"},
                2,
                true
            };
            AssertSerialization(list);
        }

        [Test]
        public void HandlesMultiTargetTypeArray()
        {
            var list = new object[]
            {
                "1",
                10.5,
                new string[] {"a", "b"},
                true,
                new StructObject { NameProp = "structName", numberField = 5 },
                5,
                4,
                3,
            };
            var json = Serialize(list);
            var types = new[] { typeof(string), typeof(decimal), typeof(string[]), typeof(bool), typeof(StructObject), typeof(int) };
            var obtained = Deserializer.Deserialize(json, types);
            Assert.AreEqual(list, obtained);

            // validate that an empty array is returned if the jsonString is null
            var emptyObtained = Deserializer.Deserialize(null, types);
            Assert.AreEqual(Array.Empty<object>(), emptyObtained);
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

            Assert.DoesNotThrow(() => Serialize(list));
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

            // use a default serializer, without references handling
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                IncludeFields = true,
                MaxDepth = int.MaxValue,
            };
            var json = JsonSerializer.Serialize(list, jsonSerializerOptions);
            object obtainedValue = null;

            Assert.DoesNotThrow(() => obtainedValue = Deserializer.Deserialize<List<object>>(json));
        }

        [Test]
        public void HandlesListWithObjectReferences()
        {
            var list = new List<object>();
            var childList = new List<object>();
            list.Add(childList);
            list.Add(childList);

            string json = string.Empty;
            List<object> obtainedValue = null;
            Assert.DoesNotThrow(() => json = Serialize(list));
            Assert.DoesNotThrow(() => obtainedValue = Deserializer.Deserialize<List<object>>(json));
            Assert.AreEqual(2, obtainedValue.Count());
            Assert.AreSame(obtainedValue[0], obtainedValue[1]);
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

            AssertSerialization(dict);
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
            AssertSerialization(dict);
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
                    binaryField = new byte[] { 0, 1, 2 },
                    referenceField = new object()
                }
            };

            var obtainedValue = SerializeAndDeserialize(obj);
            Assert.AreEqual(obj.stringField, obtainedValue.stringField);
            var child = obj.childObj;
            var obtainedChild = obtainedValue.childObj;
            Assert.AreEqual(child.binaryField, obtainedChild.binaryField);
            Assert.AreEqual(child.boolField, obtainedChild.boolField);
            Assert.AreEqual(child.dateField, obtainedChild.dateField);
            Assert.AreEqual(child.doubleField, obtainedChild.doubleField);
            Assert.AreEqual(child.intField, obtainedChild.intField);
            Assert.AreEqual(child.stringField, obtainedChild.stringField);
            Assert.AreSame(child.referenceField.GetType(), obtainedChild.referenceField.GetType());
            Assert.NotNull(obtainedChild.referenceField); // Assert.Equal fails when comparing two plain object instances, hence the Assert.NotNull
        }

        [Test]
        public void HandlesNumberAsObject()
        {
            var obtained = Deserializer.Deserialize<object>("5");
            Assert.IsInstanceOf<double>(obtained);
            Assert.AreEqual(5, obtained);
        }

        [Test]
        public void HandlesNumberArrayAsObject()
        {
            var obtained = Deserializer.Deserialize<object>("[5]");
            Assert.IsInstanceOf<object[]>(obtained);
            CollectionAssert.AreEqual(new[] { 5 }, (object[])obtained);
        }

        [Test]
        public void HandlesJsObjectAsObject()
        {
            var obtained = Deserializer.Deserialize<object>("{\"key\":5}");
            Assert.IsInstanceOf<IDictionary<string, object>>(obtained);
            CollectionAssert.AreEqual(new[] { KeyValuePair.Create("key", 5) }, (IDictionary<string, object>)obtained);
        }

        [Test]
        public void HandlesJsObjectAsExpandoObject()
        {
            var obtained = Deserializer.Deserialize<ExpandoObject>("{\"key\":5}");
            Assert.IsInstanceOf<IDictionary<string, object>>(obtained);
            CollectionAssert.AreEqual(new[] { KeyValuePair.Create("key", 5) }, obtained);
        }

        [Test]
        public void HandlesValueTypes()
        {
            var obtained = Deserializer.Deserialize<StructObject>("null");
            Assert.IsInstanceOf<StructObject>(obtained);
            Assert.AreEqual(default(StructObject), obtained);

            // when null is deserialized to a Structs array
            var obtainedNullArray = Deserializer.Deserialize<StructObject[]>("null");
            Assert.IsNull(obtainedNullArray);

            // when null appears as an element of a Structs array
            var obtainedArray = Deserializer.Deserialize<StructObject[]>("[null]");
            Assert.IsInstanceOf<StructObject[]>(obtainedArray);
            CollectionAssert.AreEqual(new[] { default(StructObject) }, obtainedArray);

            // when null appears inside an object
            var obtainedParentObj = Deserializer.Deserialize<ParentObj>("{ \"childObj\":null }");
            Assert.IsInstanceOf<ParentObj>(obtainedParentObj);
            Assert.AreEqual(default(ChildObj), obtainedParentObj.childObj);
        }

        [Test]
        public void HandlesMissingObjectFields()
        {
            Assert.DoesNotThrow(() => Deserializer.Deserialize<Person>($"{{\"Name\":\"{DataMarkers.StringMarker}student\",\"MissingField\":0}}"));
        }
    }
}
