using System.Collections.Generic;
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
            Assert.AreEqual(5, JavascriptToNativeTypeConverter.ConvertToNative<int>(5));
            Assert.AreEqual(5.1d, JavascriptToNativeTypeConverter.ConvertToNative<double>(5.1d));

            Assert.AreEqual(false, JavascriptToNativeTypeConverter.ConvertToNative<bool>(false));
            Assert.AreEqual(true, JavascriptToNativeTypeConverter.ConvertToNative<bool>(true));

            Assert.AreEqual(TestEnum.OptionB, JavascriptToNativeTypeConverter.ConvertToNative<TestEnum>(1));

            Assert.AreEqual("this is a string", JavascriptToNativeTypeConverter.ConvertToNative<string>("this is a string"));
            Assert.AreEqual("", JavascriptToNativeTypeConverter.ConvertToNative<string>(""));
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
            Assert.AreEqual(default(ChildObj), JavascriptToNativeTypeConverter.ConvertToNative<ChildObj>(null));
            Assert.AreEqual(default(int[]), JavascriptToNativeTypeConverter.ConvertToNative<int[]>(null));
        }

        [Test]
        public void DynamicObjects()
        {
            var stringDict = new Dictionary<string, object>() {
                { "a", "1" },
                { "b", "2" }
            };
            Assert.AreEqual(stringDict, JavascriptToNativeTypeConverter.ConvertToNative<dynamic>(stringDict));
        }

        [Test]
        public void Objects()
        {
            var obj = new Dictionary<string, object>()
            {
                {nameof(ChildObj.stringField), "test1"}, {nameof(ChildObj.intField), 1}
            };
            Assert.AreEqual(new ChildObj() {stringField = "test1", intField = 1},
                JavascriptToNativeTypeConverter.ConvertToNative<ChildObj>(obj));
        }

        [Test] 
        public void NestedObjects()
        {
            // struct with nested struct
            var obj = new Dictionary<string, object> 
            {
                { nameof(ParentObj.stringField), "parent" },
                { nameof(ParentObj.childObj), new Dictionary<string, object>() { { nameof(ChildObj.stringField), "child" }, { nameof(ChildObj.intField), 2 } } },
            };
            var expected = new ParentObj()
            {
                stringField = "parent",
                childObj = new ChildObj()
                {
                    stringField = "child",
                    intField = 2
                }
            };
            Assert.AreEqual(expected, JavascriptToNativeTypeConverter.ConvertToNative<ParentObj>(obj));
        }

        [Test]
        public void Arrays()
        {
            var emptyStringList = JavascriptToNativeTypeConverter.ConvertToNative<string[]>(new object[0]);
            Assert.IsInstanceOf(typeof(string[]), emptyStringList);
            CollectionAssert.AreEqual(new string[0], emptyStringList);

            var emptyIntList = JavascriptToNativeTypeConverter.ConvertToNative<int[]>(new object[0]);
            Assert.IsInstanceOf(typeof(int[]), emptyIntList);
            CollectionAssert.AreEqual(new int[0], emptyIntList);

            var emptyBoolList = JavascriptToNativeTypeConverter.ConvertToNative<bool[]>(new object[0]);
            Assert.IsInstanceOf(typeof(bool[]), emptyBoolList);
            CollectionAssert.AreEqual(new bool[0], emptyBoolList);

            var stringList = new object[] { "I", "have", "something", "to", "test", null };
            CollectionAssert.AreEqual(stringList, JavascriptToNativeTypeConverter.ConvertToNative<string[]>(stringList));

            var intList = new object[] { 1, 2, 3 };
            CollectionAssert.AreEqual(intList, JavascriptToNativeTypeConverter.ConvertToNative<int[]>(intList));

            var boolList = new object[] { true, false, true };
            CollectionAssert.AreEqual(boolList, JavascriptToNativeTypeConverter.ConvertToNative<bool[]>(boolList));

            var objectList = new object[] { 1, 2.0, "s", "string", null };
            CollectionAssert.AreEqual(objectList, JavascriptToNativeTypeConverter.ConvertToNative<object[]>(objectList));

            var structList = new dynamic[] 
            {
               new Dictionary<string, object>() { { nameof(ChildObj.stringField), "test1" }, { nameof(ChildObj.intField), 1 } },
               new Dictionary<string, object>() { { nameof(ChildObj.stringField), "test2" }, { nameof(ChildObj.intField), 2 } },
            };
            var expectedStructList = new[]
            {
                new ChildObj() { stringField = "test1", intField = 1 },
                new ChildObj() { stringField = "test2", intField = 2 },
            };
            CollectionAssert.AreEqual(expectedStructList, JavascriptToNativeTypeConverter.ConvertToNative<ChildObj[]>(structList));
        }
    }
}
