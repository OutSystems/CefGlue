using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Shared.Serialization;
using static Xilium.CefGlue.Common.Shared.Serialization.DataMarkers;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class DeserializationTests
    {
        # region DeserializeCefValue
        private void AssertDeserialization<T>(dynamic value, CefValueType valueType, Expression<Func<CefValueWrapper, T>> getValue)
        {
            var cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(v => v.GetValueType()).Returns(valueType);
            if (getValue != null)
            {
                cefValue.Setup(getValue).Returns(value);
            }
            Assert.AreEqual(value, CefValueSerialization.DeserializeCefValue(cefValue.Object));
        }

        private Mock<CefValueWrapper> BootstrapDictionaryWrapperMock(CefValueType valueType)
        {
            var cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(v => v.GetValueType()).Returns(valueType);
            return cefValue;
        }

        private Mock<ICefDictionaryValue> SimpleDictionaryMock<T>(Dictionary<string, T> baseDictionary, CefValueType valueType, Func<string, Expression<Func<ICefDictionaryValue, T>>> getValue)
        {
            var cefDictionary = new Mock<ICefDictionaryValue>();
            cefDictionary.Setup(v => v.GetKeys()).Returns(baseDictionary.Keys.ToArray());
            foreach (string key in baseDictionary.Keys)
            {
                cefDictionary.Setup(v => v.GetValueType(key)).Returns(valueType);
                cefDictionary.Setup(getValue(key)).Returns(baseDictionary[key]);
            }
            return cefDictionary;
        }

        private Mock<ICefListValue> SimpleListMock<T>(List<T> baseList, CefValueType valueType, Func<int, Expression<Func<ICefListValue, T>>> getValue)
        {
            var cefList = new Mock<ICefListValue>();
            cefList.Setup(v => v.Count).Returns(baseList.Count);
            for (int i = 0; i < baseList.Count; i++)
            {
                cefList.Setup(v => v.GetValueType(i)).Returns(valueType);
                cefList.Setup(getValue(i)).Returns(baseList[i]);
            }
            return cefList;
        }

        private IDictionary<string, object> SerializeSimpleDictionary<T>(Dictionary<string, T> baseDictionary, CefValueType valueType, Func<string,  Expression<Func<ICefDictionaryValue, T>>> getValue) {
            var cefValue = BootstrapDictionaryWrapperMock(CefValueType.Dictionary);
            cefValue.Setup(v => v.GetDictionary()).Returns(SimpleDictionaryMock<T>(baseDictionary, valueType, getValue).Object);
            return (IDictionary<string, object>)CefValueSerialization.DeserializeCefValue(cefValue.Object);
        }

        private IList<object> SerializeSimpleList<T>(List<T> baseList, CefValueType valueType, Func<int, Expression<Func<ICefListValue, T>>> getValue)
        {
            var cefValue = BootstrapDictionaryWrapperMock(CefValueType.List);
            cefValue.Setup(v => v.GetList()).Returns(SimpleListMock<T>(baseList, valueType, getValue).Object);
            return (IList<object>)CefValueSerialization.DeserializeCefValue(cefValue.Object);
        }

        [Test]
        public void DeserializeCefValue_HandlesIntegers()
        {
            var returnValue = 5;
            AssertDeserialization(returnValue, CefValueType.Int, c => c.GetInt());
        }

        [Test]
        public void DeserializeCefValue_HandlesStrings()
        {
            var returnValue = "this is a string";
            AssertDeserialization(returnValue, CefValueType.String, c => c.GetString());
        }

        [Test]
        public void DeserializeCefValue_HandlesDoubles()
        {
            var returnValue = 5d;
            AssertDeserialization(returnValue, CefValueType.Double, c => c.GetDouble());
        }

        [Test]
        public void DeserializeCefValue_HandlesDoublesWithDecimals()
        {
            var returnValue = 5.1d;
            AssertDeserialization(returnValue, CefValueType.Double, c => c.GetDouble());
        }

        [Test]
        public void DeserializeCefValue_HandlesBooleans()
        {
            var returnValue = false;
            AssertDeserialization(returnValue, CefValueType.Bool, c => c.GetBool());
        }

        [Test]
        public void DeserializeCefValue_HandlesNullStrings()
        {
            var returnValue = "";
            AssertDeserialization(returnValue, CefValueType.String, c => c.GetString());
        }

        [Test]
        public void DeserializeCefValue_HandlesNullObjects()
        {
            object returnValue = null;
            AssertDeserialization<object>(returnValue, CefValueType.Null, null);
        }

        [Test]
        public void DeserializeCefValue_HandlesBinaries()
        {
            var returnValue = new byte[] { 100, 150, 254 };
            var cefValue = new Mock<CefValueWrapper>();
            var byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns((new byte[] { 1 }.Concat(returnValue)).ToArray());
            cefValue.Setup(v => v.GetValueType()).Returns(CefValueType.Binary);
            cefValue.Setup(v => v.GetBinary()).Returns(byteArray.Object);
            Assert.AreEqual(returnValue, CefValueSerialization.DeserializeCefValue(cefValue.Object));
        }

        [Test]
        public void DeserializeCefValue_HandlesSimpleDictionaries()
        {
            var returnValue = new Dictionary<string, int>()
            {
                { "first", 1 },
                { "second", 2 },
                { "third", 3 }
            };

            CollectionAssert.AreEqual(returnValue, SerializeSimpleDictionary<int>(returnValue, CefValueType.Int, k => v => v.GetInt(k)));
        }

        [Test]
        public void DeserializeCefValue_HandlesNestedDictionaries()
        {
            var returnValue = new Dictionary<string, Dictionary<string, double>>()
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
            var cefValue = BootstrapDictionaryWrapperMock(CefValueType.Dictionary);

            var cefOuterDictionary = new Mock<ICefDictionaryValue>();
            cefOuterDictionary.Setup(v => v.GetKeys()).Returns(returnValue.Keys.ToArray());
            foreach (string key in returnValue.Keys)
            {
                cefOuterDictionary.Setup(v => v.GetValueType(key)).Returns(CefValueType.Dictionary);
                cefOuterDictionary.Setup(v => v.GetDictionary(key))
                    .Returns(SimpleDictionaryMock<double>(returnValue[key], CefValueType.Double, k => v => v.GetDouble(k)).Object);
            }
            cefValue.Setup(v => v.GetDictionary()).Returns(cefOuterDictionary.Object);
            CollectionAssert.AreEqual(returnValue, (IDictionary<string, object>)CefValueSerialization.DeserializeCefValue(cefValue.Object));
        }

        [Test]
        public void DeserializeCefValue_HandlesNestedComplexDictionaries()
        {
            var returnValue = new Dictionary<string, Dictionary<string, List<double>>>()
            {
                { "first", new Dictionary<string, List<double>>() {
                    { "first_first", new List<double>() { 1d, 2d, 3d } },
                    { "first_second", new List<double>() { } },
                    { "first_third", new List<double>() { 4d, 5d, 6d, 7d } },
                }},
                { "second", new Dictionary<string, List<double>>() {
                    { "second_first", new List<double>() { 8d, 9d } },
                    { "second_second", new List<double>() { 10d } },
                    { "second_third", new List<double>() { } },
                }},
                { "third", new Dictionary<string, List<double>>() {
                    { "third_first", new List<double>() { 11.1d, 12.2d, 13.3d } },
                    { "third_second", new List<double>() { } },
                    { "third_third", new List<double>() { 14d, 15d, 16d } },
                }}
            };
            var cefValue = BootstrapDictionaryWrapperMock(CefValueType.Dictionary);

            var cefOuterDictionary = new Mock<ICefDictionaryValue>();
            cefOuterDictionary.Setup(v => v.GetKeys()).Returns(returnValue.Keys.ToArray());
            foreach (string key in returnValue.Keys)
            {
                cefOuterDictionary.Setup(v => v.GetValueType(key)).Returns(CefValueType.Dictionary);
                var cefInnerDictionary = new Mock<ICefDictionaryValue>();
                cefInnerDictionary.Setup(v => v.GetKeys()).Returns(returnValue[key].Keys.ToArray());
                foreach (string innerkey in returnValue[key].Keys)
                {
                    cefInnerDictionary.Setup(v => v.GetValueType(innerkey)).Returns(CefValueType.List);
                    cefInnerDictionary.Setup(v => v.GetList(innerkey))
                        .Returns(SimpleListMock<double>(returnValue[key][innerkey], CefValueType.Double, i => v => v.GetDouble(i)).Object);
                }
                cefOuterDictionary.Setup(v => v.GetDictionary(key)).Returns(cefInnerDictionary.Object);
            }
            cefValue.Setup(v => v.GetDictionary()).Returns(cefOuterDictionary.Object);
            CollectionAssert.AreEqual(returnValue, (IDictionary<string, object>)CefValueSerialization.DeserializeCefValue(cefValue.Object));
        }

        [Test]
        public void DeserializeCefValue_HandlesSimpleLists()
        {
            var returnValue = new List<int>() { 1, 2, 3 };
            CollectionAssert.AreEqual(returnValue, SerializeSimpleList<int>(returnValue, CefValueType.Int, i => v => v.GetInt(i)));
        }

        [Test]
        public void DeserializeCefValue_HandlesNestedLists()
        {
            var returnValue = new List<List<double>>()
            {
                new List<double>() { 1d, 2d, 3d },
                new List<double>() { 4d, 5d, 6d, 7d },
                new List<double>() { 9d },
            };

            var cefValue = BootstrapDictionaryWrapperMock(CefValueType.List);

            var cefOuterList = new Mock<ICefListValue>();
            cefOuterList.Setup(v => v.Count).Returns(returnValue.Count);
            for (int i = 0; i < returnValue.Count; i++)
            {
                cefOuterList.Setup(v => v.GetValueType(i)).Returns(CefValueType.List);
                cefOuterList.Setup(v => v.GetList(i)).Returns(SimpleListMock<double>(returnValue[i], CefValueType.Double, j => v => v.GetDouble(j)).Object);
            }
            cefValue.Setup(v => v.GetList()).Returns(cefOuterList.Object);
            CollectionAssert.AreEqual(returnValue, (IList<object>)CefValueSerialization.DeserializeCefValue(cefValue.Object));
        }

        [Test]
        public void DeserializeCefValue_HandlesComplexNestedLists()
        {
            var returnValue = new List<List<Dictionary<string, string>>>()
            {
                new List<Dictionary<string, string>>() { 
                    new Dictionary<string, string>() {
                        { "1st", "1st" },
                        { "2nd", "2nd" },
                        { "3rd", "3rd" },
                    },
                    new Dictionary<string, string>() {
                        { "4th", "4th" },
                    },
                },
                new List<Dictionary<string, string>>() { 
                    new Dictionary<string, string>() { }
                },
                new List<Dictionary<string, string>>() {
                    new Dictionary<string, string>() {
                        { "5th", "5th" },
                        { "6th", "6th" },
                        { "7th", "7th" },
                        { "8th", "8th" },
                    },
                    new Dictionary<string, string>() {
                        { "9th", "9th" },
                    },
                    new Dictionary<string, string>() { },
                    new Dictionary<string, string> {
                        { "10th", "10th" },
                        { "11th", "11th" },
                    },
                },
            };

            var cefValue = BootstrapDictionaryWrapperMock(CefValueType.List);

            var cefOuterList = new Mock<ICefListValue>();
            cefOuterList.Setup(v => v.Count).Returns(returnValue.Count);
            for (int i = 0; i < returnValue.Count; i++)
            {
                cefOuterList.Setup(v => v.GetValueType(i)).Returns(CefValueType.List);
                var cefInnerList = new Mock<ICefListValue>();
                cefInnerList.Setup(v => v.Count).Returns(returnValue[i].Count);
                for (int j = 0; j < returnValue[i].Count; j++)
                {
                    cefInnerList.Setup(v => v.GetValueType(j)).Returns(CefValueType.Dictionary);
                    cefInnerList.Setup(v => v.GetDictionary(j))
                        .Returns(SimpleDictionaryMock<string>(returnValue[i][j], CefValueType.String, k => v => v.GetString(k)).Object);
                }
                cefOuterList.Setup(v => v.GetList(i)).Returns(cefInnerList.Object);
            }
            cefValue.Setup(v => v.GetList()).Returns(cefOuterList.Object);
            CollectionAssert.AreEqual(returnValue, (IList<object>)CefValueSerialization.DeserializeCefValue(cefValue.Object));
        }

        #endregion

        #region FromCefBinary
        [Test]
        public void FromCefBinary_HandlesEmptyByteArrays()
        {
            var returnValue = new byte[0];
            var byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns(returnValue);
            Assert.AreEqual(returnValue, CefValueSerialization.FromCefBinary(byteArray.Object, out var kind));
            Assert.AreEqual(BinaryMagicBytes.Binary, kind);
        }

        [Test]
        public void FromCefBinary_HandlesByteArrays()
        {
            var returnValue = new byte[] { 100, 150, 254 };
            var byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns((new byte[] { 1 }.Concat(returnValue)).ToArray());
            Assert.AreEqual(returnValue, CefValueSerialization.FromCefBinary(byteArray.Object, out var kind));
            Assert.AreEqual(BinaryMagicBytes.Binary, kind);
        }

        [Test]
        public void FromCefBinary_ThrowsExceptionOnUnknownByteTypes()
        {
            var returnValue = new byte[] { 10, 100, 150, 254 };
            var byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns(returnValue);
            Assert.Throws<InvalidOperationException>(() => CefValueSerialization.FromCefBinary(byteArray.Object, out var kind));
        }

        [Test]
        public void FromCefBinary_HandlesDateTimes()
        {
            var dateTime = DateTime.Now;
            var returnValue = BitConverter.GetBytes(dateTime.Ticks);
            var byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns((new byte[] { 0 }.Concat(returnValue)).ToArray());
            Assert.AreEqual(dateTime, CefValueSerialization.FromCefBinary(byteArray.Object, out var kind));
            Assert.AreEqual(BinaryMagicBytes.DateTime, kind);
        }
        #endregion
    }
}
