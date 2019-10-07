using NUnit.Framework;
using Xilium.CefGlue.Common.Helpers;

namespace CefGlue.Tests.Serialization
{
    public class JavascriptToNativeTypeConverterTests
    {
        private enum TestEnum
        {
            OptionA,
            OptionB
        }

        [Test]
        public void BasicTypes()
        {
            Assert.AreEqual(5, JavascriptToNativeTypeConverter.ConvertToNative(5, typeof(int)));
            Assert.AreEqual(5.1d, JavascriptToNativeTypeConverter.ConvertToNative(5.1d, typeof(double)));

            Assert.AreEqual(false, JavascriptToNativeTypeConverter.ConvertToNative(false, typeof(bool)));
            Assert.AreEqual(true, JavascriptToNativeTypeConverter.ConvertToNative(true, typeof(bool)));

            Assert.AreEqual(TestEnum.OptionB, JavascriptToNativeTypeConverter.ConvertToNative(1, typeof(TestEnum)));

            Assert.AreEqual("this is a string", JavascriptToNativeTypeConverter.ConvertToNative("this is a string", typeof(string)));

            Assert.AreEqual(null, JavascriptToNativeTypeConverter.ConvertToNative(null, typeof(string)));
        }

        [Test]
        public void DoubleCastToInt()
        {
            Assert.AreEqual(5d, JavascriptToNativeTypeConverter.ConvertToNative(5, typeof(double)));
        }

        [Test]
        public void DecimalsAreNotLost()
        {
            Assert.AreEqual(5d, JavascriptToNativeTypeConverter.ConvertToNative(5, typeof(double)));
        }

        [Test]
        public void Dictionaries()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void Lists()
        {
            Assert.Fail("TODO");
        }

        [Test]
        public void Arrays()
        {
            Assert.Fail("TODO");
        }
    }
}
