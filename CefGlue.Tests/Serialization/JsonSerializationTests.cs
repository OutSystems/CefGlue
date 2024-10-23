using NUnit.Framework;

using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json;

using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization.Json;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class JsonSerializationTests
    {
        private static readonly Messaging messaging = Messaging.Json;

        private static ObjectType SerializeAndDeserialize<ObjectType>(ObjectType value)
        {
            var json = messaging.Serialize(value);
            var result = messaging.Deserialize<ObjectType>(json);
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

            Assert.DoesNotThrow(() => obtainedValue = messaging.Deserialize<List<object>>(Encoding.UTF8.GetBytes(json)));
        }

        [Test]
        public void HandlesNumberAsObject()
        {
            var obtained = messaging.Deserialize<object>(Encoding.UTF8.GetBytes("5"));
            Assert.IsInstanceOf<double>(obtained);
            Assert.AreEqual(5, obtained);
        }

        [Test]
        public void HandlesNumberArrayAsObject()
        {
            var obtained = messaging.Deserialize<object>(Encoding.UTF8.GetBytes("[5]"));
            Assert.IsInstanceOf<object[]>(obtained);
            CollectionAssert.AreEqual(new[] { 5 }, (object[])obtained);
        }

        [Test]
        public void HandlesJsObjectAsObject()
        {
            var obtained = messaging.Deserialize<object>(Encoding.UTF8.GetBytes("{\"key\":5}"));
            Assert.IsInstanceOf<IDictionary<string, object>>(obtained);
            CollectionAssert.AreEqual(new[] { KeyValuePair.Create("key", 5) }, (IDictionary<string, object>)obtained);
        }

        [Test]
        public void HandlesJsObjectAsExpandoObject()
        {
            var obtained = messaging.Deserialize<ExpandoObject>(Encoding.UTF8.GetBytes("{\"key\":5}"));
            Assert.IsInstanceOf<IDictionary<string, object>>(obtained);
            CollectionAssert.AreEqual(new[] { KeyValuePair.Create("key", 5) }, obtained);
        }

        [Test]
        public void HandlesValueTypes()
        {
            var obtained = messaging.Deserialize<StructObject>(Encoding.UTF8.GetBytes("null"));
            Assert.IsInstanceOf<StructObject>(obtained);
            Assert.AreEqual(default(StructObject), obtained);

            // when null is deserialized to a Structs array
            var obtainedNullArray = messaging.Deserialize<StructObject[]>(Encoding.UTF8.GetBytes("null"));
            Assert.IsNull(obtainedNullArray);

            // when null appears as an element of a Structs array
            var obtainedArray = messaging.Deserialize<StructObject[]>(Encoding.UTF8.GetBytes("[null]"));
            Assert.IsInstanceOf<StructObject[]>(obtainedArray);
            CollectionAssert.AreEqual(new[] { default(StructObject) }, obtainedArray);

            // when null appears inside an object
            var obtainedParentObj = messaging.Deserialize<ParentObj>(Encoding.UTF8.GetBytes("{ \"childObj\":null }"));
            Assert.IsInstanceOf<ParentObj>(obtainedParentObj);
            Assert.AreEqual(default(ChildObj), obtainedParentObj.childObj);
        }

        [Test]
        public void HandlesMissingObjectFields()
        {
            Assert.DoesNotThrow(() => messaging.Deserialize<Person>(Encoding.UTF8.GetBytes($"{{\"Name\":\"{DataMarkers.StringMarker}student\",\"MissingField\":0}}")));
        }
    }
}
