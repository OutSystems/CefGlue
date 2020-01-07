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

        private struct TestStructure
        {

        }

        [Test]
        public void BasicTypes()
        {
            Assert.AreEqual(5, JavascriptToNativeTypeConverter.ConvertToNative<int>(5));
            Assert.AreEqual(5.1d, JavascriptToNativeTypeConverter.ConvertToNative<double>(5.1d));

            Assert.AreEqual(false, JavascriptToNativeTypeConverter.ConvertToNative<bool>(false));
            Assert.AreEqual(true, JavascriptToNativeTypeConverter.ConvertToNative<bool>(true));

            Assert.AreEqual(TestEnum.OptionB, JavascriptToNativeTypeConverter.ConvertToNative<TestEnum>(1));

            Assert.AreEqual("this is a string", JavascriptToNativeTypeConverter.ConvertToNative<string>("this is a string"));
        }

        [Test]
        public void DoubleCastToInt()
        {
            Assert.AreEqual(5d, JavascriptToNativeTypeConverter.ConvertToNative<double>(5));
        }

        [Test]
        public void DecimalsAreNotLost()
        {
            Assert.AreEqual(5d, JavascriptToNativeTypeConverter.ConvertToNative<double>(5));
        }

        [Test]
        public void NullReturnsTypeDefaultValue()
        {
            Assert.AreEqual(default(string), JavascriptToNativeTypeConverter.ConvertToNative<string>(null));
            Assert.AreEqual(default(int), JavascriptToNativeTypeConverter.ConvertToNative<int>(null));
            Assert.AreEqual(default(bool), JavascriptToNativeTypeConverter.ConvertToNative<bool>(null));
            Assert.AreEqual(default(TestEnum), JavascriptToNativeTypeConverter.ConvertToNative<TestEnum>(null));
            Assert.AreEqual(default(TestStructure), JavascriptToNativeTypeConverter.ConvertToNative<TestStructure>(null));
            Assert.AreEqual(default(int[]), JavascriptToNativeTypeConverter.ConvertToNative<int[]>(null));
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
