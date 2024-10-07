using NUnit.Framework;
using System;
using System.Collections.Generic;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class SerializationTests
    {
        private static ObjectType SerializeAndDeserialize<ObjectType>(ObjectType value, MessageContextType messageContextType)
        {
            var messageContext = MessageContextTypeHelper.GetMessageContext(messageContextType);
            var bytes = messageContext.Serialize(value);
            var result = messageContext.Deserialize<ObjectType>(bytes);
            return result;
        }

        private static ObjectType AssertSerialization<ObjectType>(ObjectType value,
            MessageContextType messageContextType, bool assertEquality = true)
        {
            var obtainedValue = SerializeAndDeserialize(value, messageContextType);
            Assert.AreSame(value?.GetType(), obtainedValue?.GetType());
            if (assertEquality)
            {
                Assert.AreEqual(value, obtainedValue);
            }

            return obtainedValue;
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesNullObject(MessageContextType messageContextType)
        {
            AssertSerialization((object)null, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack,
            Ignore = "Msgpack serializer insists to deserialize plain objects to dictionaries.")]
        public void HandlesPlainObject(MessageContextType messageContextType)
        {
            var obtainedValue = AssertSerialization(new object(), messageContextType, assertEquality: false);
            Assert.NotNull(obtainedValue);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesBooleans(MessageContextType messageContextType)
        {
            AssertSerialization(true, messageContextType);
            AssertSerialization(false, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesSignedIntegers16(MessageContextType messageContextType)
        {
            AssertSerialization(Int16.MaxValue, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesSignedIntegers32(MessageContextType messageContextType)
        {
            AssertSerialization(Int32.MaxValue, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesSignedIntegers64(MessageContextType messageContextType)
        {
            AssertSerialization(Int64.MaxValue, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesUnsignedIntegers16(MessageContextType messageContextType)
        {
            AssertSerialization(UInt16.MinValue, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesUnsignedIntegers32(MessageContextType messageContextType)
        {
            AssertSerialization(UInt32.MinValue, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesUnsignedIntegers64(MessageContextType messageContextType)
        {
            AssertSerialization(UInt64.MinValue, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesBytes(MessageContextType messageContextType)
        {
            AssertSerialization((byte)12, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesStrings(MessageContextType messageContextType)
        {
            AssertSerialization("this is a string", messageContextType);
            AssertSerialization("", messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesStringsWithSpecialChars(MessageContextType messageContextType)
        {
            AssertSerialization("日本語組版処理の", messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesChars(MessageContextType messageContextType)
        {
            AssertSerialization('c', messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesDoubles(MessageContextType messageContextType)
        {
            AssertSerialization(10.5d, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesFloats(MessageContextType messageContextType)
        {
            AssertSerialization(10.5f, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesDecimals(MessageContextType messageContextType)
        {
            AssertSerialization(10.5m, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesBinaries(MessageContextType messageContextType)
        {
            AssertSerialization((byte[]) [0, 1, 2, 3], messageContextType);
            AssertSerialization((byte[]) [], messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesDateTimes(MessageContextType messageContextType)
        {
            var date = new DateTime(2000, 1, 31, 15, 00, 10);
            AssertSerialization(date, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesDictionaryOfPrimitiveValueTypeCollection(MessageContextType messageContextType)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>
            {
                ["first"] = 1, ["second"] = 2, ["third"] = 3,
            };
            AssertSerialization(dict, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesArrays(MessageContextType messageContextType)
        {
            string[] list = ["1", "2"];
            AssertSerialization(list, messageContextType);
            AssertSerialization(new string[0], messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesArraysOfStructs(MessageContextType messageContextType)
        {

            StructObject[] list = [new StructObject("first", 1), new StructObject("second", 2)];
            AssertSerialization(list, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesLists(MessageContextType messageContextType)
        {
            List<string> list = ["1", "2"];
            AssertSerialization(list, messageContextType);
            AssertSerialization(new List<string>(), messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesNestedLists(MessageContextType messageContextType)
        {
            List<List<string>> list =
            [
                ["1", "2"],
                ["3", "4"]
            ];
            AssertSerialization(list, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesListsOfObjects(MessageContextType messageContextType)
        {
            List<Dictionary<string, string>> list =
            [
                new Dictionary<string, string> { ["first"] = "1", ["second"] = "2", },
                new Dictionary<string, string> { ["third"] = "3", ["fourth"] = "4", },
            ];
            AssertSerialization(list, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesObjectsArray(MessageContextType messageContextType)
        {
            object[] array =
            [
                "1",
                null,
                new string[] { "a", "b" },
                2,
                true
            ];
            AssertSerialization(array, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesMultiTargetTypeArray(MessageContextType messageContextType)
        {
            MessageContext messageContext = MessageContextTypeHelper.GetMessageContext(messageContextType);

            object[] list =
            [
                "1",
                10.5m,
                (string[]) ["a", "b"],
                true,
                new StructObject { NameProp = "structName", numberField = 5 },
                (int[])
                [
                    5,
                    4,
                    3,
                ]
            ];
            var bytes = messageContext.Serialize(list);
            Type[] types =
                [typeof(string), typeof(decimal), typeof(string[]), typeof(bool), typeof(StructObject), typeof(int[])];
            var obtained = messageContext.Deserialize(bytes, types);
            Assert.AreEqual(list, obtained);

            // TODO: Ensure removed support for null input is handled elsewhere
            // validate that an empty array is returned if the jsonString is null
            //var emptyObtained = messageContext.Deserialize(null, types);
            //Assert.AreEqual(Array.Empty<object>(), emptyObtained);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesSerializationOfDeepListsWith250Levels(MessageContextType messageContextType)
        {
            MessageContext messageContext = MessageContextTypeHelper.GetMessageContext(messageContextType);

            var list = new List<object>();
            var child = new List<object>();
            list.Add(child);

            for (var i = 0; i < 250; i++)
            {
                var nestedChild = new List<object>();
                child.Add(nestedChild);
                child = nestedChild;
            }

            Assert.DoesNotThrow(() => messageContext.Serialize(list));
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesListWithObjectReferences(MessageContextType messageContextType)
        {
            MessageContext messageContext = MessageContextTypeHelper.GetMessageContext(messageContextType);

            List<object> list = [];
            List<object> childList = [];
            list.Add(childList);
            list.Add(childList);

            byte[] bytes = [];
            List<object> obtainedValue = null;
            Assert.DoesNotThrow(() => bytes = messageContext.Serialize(list));
            Assert.DoesNotThrow(() => obtainedValue = messageContext.Deserialize<List<object>>(bytes));
            Assert.AreEqual(2, obtainedValue.Count);
            Assert.AreSame(obtainedValue[0], obtainedValue[1]);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesSimpleDictionaries(MessageContextType messageContextType)
        {
            var dict = new Dictionary<string, object>
            {
                ["first"] = 1,
                ["second"] = "string",
                ["third"] = true,
                ["fourth"] = new DateTime(),
                ["fifth"] = (byte[]) [0, 1, 2],
                ["sixth"] = null,
                ["seventh"] = 7.0d
            };

            AssertSerialization(dict, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack)]
        public void HandlesNestedDictionaries(MessageContextType messageContextType)
        {
            var dict = new Dictionary<string, Dictionary<string, double>>()
            {
                ["first"] =
                    new Dictionary<string, double>
                    {
                        ["first_first"] = 1d, ["first_second"] = 2d, ["first_third"] = 3d,
                    },
                ["second"] =
                    new Dictionary<string, double>
                    {
                        ["second_first"] = 4d, ["second_second"] = 5d, ["second_third"] = 6d,
                    },
                ["third"] = new Dictionary<string, double>
                {
                    ["third_first"] = 7d, ["third_second"] = 8d, ["third_third"] = 9d,
                }
            };
            AssertSerialization(dict, messageContextType);
        }

        [TestCase(MessageContextType.Json)]
        [TestCase(MessageContextType.MsgPack,
            Ignore = "Msgpack serializer insists to deserialize plain objects to dictionaries.")]
        public void HandlesObjects(MessageContextType messageContextType)
        {
            var obj = new ParentObj
            {
                stringField = "parent",
                childObj = new ChildObj
                {
                    stringField = "child",
                    intField = 1,
                    boolField = true,
                    dateField = DateTime.Now.ToUniversalTime(),
                    doubleField = 5.5,
                    binaryField = [0, 1, 2],
                    referenceField = new object()
                }
            };

            var obtainedValue = SerializeAndDeserialize(obj, messageContextType);
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
            Assert.NotNull(obtainedChild
                .referenceField); // Assert.Equal fails when comparing two plain object instances, hence the Assert.NotNull
        }
    }
}