using NUnit.Framework;
using System;
using System.Collections.Generic;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class SerializationTests
    {
        private static ObjectType SerializeAndDeserialize<ObjectType>(ObjectType value, MessagingType messagingType)
        {
            Messaging messaging = GetMessaging(messagingType);
            var bytes = messaging.Serialize(value);
            var result = messaging.Deserialize<ObjectType>(bytes);
            return result;
        }

        private static ObjectType AssertSerialization<ObjectType>(ObjectType value,
            MessagingType messagingType, bool assertEquality = true)
        {
            var obtainedValue = SerializeAndDeserialize(value, messagingType);
            Assert.AreSame(value?.GetType(), obtainedValue?.GetType());
            if (assertEquality)
            {
                Assert.AreEqual(value, obtainedValue);
            }

            return obtainedValue;
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesNullObject(MessagingType messageContextType)
        {
            AssertSerialization((object)null, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack,
            Ignore = "Msgpack serializer insists to deserialize plain objects to dictionaries.")]
        public void HandlesPlainObject(MessagingType messageContextType)
        {
            var obtainedValue = AssertSerialization(new object(), messageContextType, assertEquality: false);
            Assert.NotNull(obtainedValue);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesBooleans(MessagingType messageContextType)
        {
            AssertSerialization(true, messageContextType);
            AssertSerialization(false, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesSignedIntegers16(MessagingType messageContextType)
        {
            AssertSerialization(Int16.MaxValue, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesSignedIntegers32(MessagingType messageContextType)
        {
            AssertSerialization(Int32.MaxValue, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesSignedIntegers64(MessagingType messageContextType)
        {
            AssertSerialization(Int64.MaxValue, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesUnsignedIntegers16(MessagingType messageContextType)
        {
            AssertSerialization(UInt16.MinValue, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesUnsignedIntegers32(MessagingType messageContextType)
        {
            AssertSerialization(UInt32.MinValue, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesUnsignedIntegers64(MessagingType messageContextType)
        {
            AssertSerialization(UInt64.MinValue, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesBytes(MessagingType messageContextType)
        {
            AssertSerialization((byte)12, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesStrings(MessagingType messageContextType)
        {
            AssertSerialization("this is a string", messageContextType);
            AssertSerialization("", messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesStringsWithSpecialChars(MessagingType messageContextType)
        {
            AssertSerialization("日本語組版処理の", messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesChars(MessagingType messageContextType)
        {
            AssertSerialization('c', messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesDoubles(MessagingType messageContextType)
        {
            AssertSerialization(10.5d, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesFloats(MessagingType messageContextType)
        {
            AssertSerialization(10.5f, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesDecimals(MessagingType messageContextType)
        {
            AssertSerialization(10.5m, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesBinaries(MessagingType messageContextType)
        {
            AssertSerialization((byte[]) [0, 1, 2, 3], messageContextType);
            AssertSerialization((byte[]) [], messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesDateTimes(MessagingType messageContextType)
        {
            var date = new DateTime(2000, 1, 31, 15, 00, 10);
            AssertSerialization(date, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesDictionaryOfPrimitiveValueTypeCollection(MessagingType messageContextType)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>
            {
                ["first"] = 1, ["second"] = 2, ["third"] = 3,
            };
            AssertSerialization(dict, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesArrays(MessagingType messageContextType)
        {
            string[] list = ["1", "2"];
            AssertSerialization(list, messageContextType);
            AssertSerialization(new string[0], messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesArraysOfStructs(MessagingType messageContextType)
        {

            StructObject[] list = [new StructObject("first", 1), new StructObject("second", 2)];
            AssertSerialization(list, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesLists(MessagingType messageContextType)
        {
            List<string> list = ["1", "2"];
            AssertSerialization(list, messageContextType);
            AssertSerialization(new List<string>(), messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesNestedLists(MessagingType messageContextType)
        {
            List<List<string>> list =
            [
                ["1", "2"],
                ["3", "4"]
            ];
            AssertSerialization(list, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesListsOfObjects(MessagingType messageContextType)
        {
            List<Dictionary<string, string>> list =
            [
                new Dictionary<string, string> { ["first"] = "1", ["second"] = "2", },
                new Dictionary<string, string> { ["third"] = "3", ["fourth"] = "4", },
            ];
            AssertSerialization(list, messageContextType);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesObjectsArray(MessagingType messageContextType)
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

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesMultiTargetTypeArray(MessagingType messagingType)
        {
            Messaging messaging = GetMessaging(messagingType);

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
            var bytes = messaging.Serialize(list);

            Type[] types =
                [typeof(string), typeof(decimal), typeof(string[]), typeof(bool), typeof(StructObject), typeof(int[])];
            var obtained = messaging.Deserialize(bytes, types);
            Assert.AreEqual(list, obtained);

            // TODO: Ensure removed support for null input is handled elsewhere
            // validate that an empty array is returned if the jsonString is null
            //var emptyObtained = messageContext.Deserialize(null, types);
            //Assert.AreEqual(Array.Empty<object>(), emptyObtained);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesSerializationOfDeepListsWith250Levels(MessagingType messagingType)
        {
            Messaging messaging = GetMessaging(messagingType);

            var list = new List<object>();
            var child = new List<object>();
            list.Add(child);

            for (var i = 0; i < 250; i++)
            {
                var nestedChild = new List<object>();
                child.Add(nestedChild);
                child = nestedChild;
            }

            Assert.DoesNotThrow(() => messaging.Serialize(list));
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesListWithObjectReferences(MessagingType messagingType)
        {
            Messaging messaging = GetMessaging(messagingType);

            List<object> list = [];
            List<object> childList = [];
            list.Add(childList);
            list.Add(childList);

            byte[] bytes = [];
            List<object> obtainedValue = null;
            Assert.DoesNotThrow(() => bytes = messaging.Serialize(list));
            Assert.DoesNotThrow(() => obtainedValue = messaging.Deserialize<List<object>>(bytes));
            Assert.AreEqual(2, obtainedValue.Count);
            Assert.AreSame(obtainedValue[0], obtainedValue[1]);
        }

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesSimpleDictionaries(MessagingType messageContextType)
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

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack)]
        public void HandlesNestedDictionaries(MessagingType messageContextType)
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

        [TestCase(MessagingType.Json)]
        [TestCase(MessagingType.MsgPack,
            Ignore = "Msgpack serializer insists to deserialize plain objects to dictionaries.")]
        public void HandlesObjects(MessagingType messageContextType)
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

        private static Messaging GetMessaging(MessagingType messagingType) => messagingType switch
        {
            MessagingType.Json => Messaging.Json,
            MessagingType.MsgPack => Messaging.MsgPack,
            _ => throw new ArgumentException($"Invalid MessageContextType argument: {messagingType}")
        };
    }
}