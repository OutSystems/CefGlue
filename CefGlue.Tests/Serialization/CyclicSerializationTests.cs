using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class CyclicSerializationTests
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
        public void HandlesListWithObjectReferences()
        {
            var list = new List<object>();
            var childList = new List<object>();
            list.Add(childList);
            list.Add(childList);

            byte[] bytes = Array.Empty<byte>();
            List<object> obtainedValue = null;
            Assert.DoesNotThrow(() => bytes = messaging.Serialize(list));
            Assert.DoesNotThrow(() => obtainedValue = messaging.Deserialize<List<object>>(bytes));
            Assert.AreEqual(2, obtainedValue.Count());
            Assert.AreSame(obtainedValue[0], obtainedValue[1]);
        }

    }
}
