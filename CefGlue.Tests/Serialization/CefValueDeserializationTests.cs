using NUnit.Framework;
using Xilium.CefGlue.Common.Serialization;

namespace CefGlue.Tests.Serialization
{
    public class CefValueDeserializationTests
    {
        [Test]
        public void BasicTypes()
        {
            Assert.AreEqual(5, CefValueSerialization.DeserializeCefValue(new TestCefValueWrapper(5)));
            Assert.AreEqual(5d, CefValueSerialization.DeserializeCefValue(new TestCefValueWrapper(5d)));
            Assert.AreEqual(5.1d, CefValueSerialization.DeserializeCefValue(new TestCefValueWrapper(5.1d)));

            Assert.AreEqual(false, CefValueSerialization.DeserializeCefValue(new TestCefValueWrapper(false)));
            Assert.AreEqual(true, CefValueSerialization.DeserializeCefValue(new TestCefValueWrapper(true)));

            Assert.AreEqual("this is a string", CefValueSerialization.DeserializeCefValue(new TestCefValueWrapper("this is a string")));

            Assert.AreEqual(null, CefValueSerialization.DeserializeCefValue(new TestCefValueWrapper(null)));
        }

        [Test]
        public void NullStringsAreCoercedToEmptyStrings()
        {
            Assert.AreEqual("", CefValueSerialization.DeserializeCefValue(TestCefValueWrapper.NullString));
        }

        [Test]
        public void Lists()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void Dictionaries()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void Binaries()
        {
            Assert.Fail("TODO");
        }
    }
}
