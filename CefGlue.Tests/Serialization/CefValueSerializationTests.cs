using NUnit.Framework;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Serialization;

namespace CefGlue.Tests.Serialization
{
    public class CefValueSerializationTests
    {
        //[Test]
        //public void BasicTypes()
        //{
        //    TestCefValueWrapper SerializeValue(object value)
        //    {
        //        var valueWrapper = new TestCefValueWrapper();
        //        CefValueSerialization.Serialize(value, valueWrapper);
        //        return valueWrapper;
        //    }
            
        //    Assert.AreEqual(5, SerializeValue(5).GetInt());

        //    Assert.AreEqual(5d, SerializeValue(5d).GetDouble());

        //    Assert.AreEqual(5.1d, SerializeValue(5.1d).GetDouble());

        //    Assert.AreEqual(false, SerializeValue(false).GetBool());

        //    Assert.AreEqual(true, SerializeValue(true).GetBool());

        //    Assert.AreEqual("a", SerializeValue('a').GetString());

        //    Assert.AreEqual("this is a string", SerializeValue("this is a string").GetString());

        //    Assert.AreEqual(CefValueType.Null, SerializeValue(null).GetValueType());
        //}

        [Test]
        public void Lists()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void ListsWithCycles()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void Dictionaries()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void DictionariesWithCycles()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void Binaries()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void DateTimes()
        {
            Assert.Fail("TODO");
        }
    }
}
