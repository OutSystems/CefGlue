using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        public void HandlesCyclicDictionaryReferencesWithException()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("first", dict);
            
            var cefValue = new CefTestValue();
            Assert.Throws<InvalidOperationException>(() => Serialize(dict, cefValue));
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
        public void HandlesCyclicListReferencesWithException()
        {
            var list = new List<object>();
            list.Add(list);
            
            var cefValue = new CefTestValue();
            Assert.Throws<InvalidOperationException>(() => Serialize(list, cefValue));
        }
        
        [Test]
        public void HandlesDeepStructuresWith250Levels()
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

            AssertSerialization(list, CefValueType.String);
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
