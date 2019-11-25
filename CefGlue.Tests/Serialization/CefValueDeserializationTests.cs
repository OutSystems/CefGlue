using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Serialization;
using static Xilium.CefGlue.Common.Serialization.CefValueSerialization;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class DeserializationTests
    {
        # region DeserializeCefValue
        private void AssertDeserialization<T>(dynamic value, CefValueType valueType, Expression<Func<CefValueWrapper, T>> getValue)
        {
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(v => v.GetValueType()).Returns(valueType);
            if (getValue != null)
            {
                cefValue.Setup(getValue).Returns(value);
            }
            Assert.AreEqual(value, CefValueSerialization.DeserializeCefValue(cefValue.Object));
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
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            Mock<ICefBinaryValue> byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns((new byte[] { 1 }.Concat(returnValue)).ToArray());
            cefValue.Setup(v => v.GetValueType()).Returns(CefValueType.Binary);
            cefValue.Setup(v => v.GetBinary()).Returns(byteArray.Object);
            Assert.AreEqual(returnValue, CefValueSerialization.DeserializeCefValue(cefValue.Object));
        }

        [Test]
        public void DeserializeCefValue_HandlesSimpleDictionary()
        {
            var returnValue = new Dictionary<string, int>()
            {
                { "first", 1 },
                { "second", 2 },
                { "third", 3 }
            };
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            Mock<ICefDictionaryValue> cefDictionary = new Mock<ICefDictionaryValue>();
            cefDictionary.Setup(v => v.GetKeys()).Returns(returnValue.Keys.ToArray());
            foreach (string key in returnValue.Keys)
            {
                cefDictionary.Setup(v => v.GetValueType(key)).Returns(CefValueType.Int);
                cefDictionary.Setup(v => v.GetInt(key)).Returns(returnValue[key]);
            }
            cefValue.Setup(v => v.GetValueType()).Returns(CefValueType.Dictionary);
            cefValue.Setup(v => v.GetDictionary()).Returns(cefDictionary.Object);
            var ret = (IDictionary<string, object>)CefValueSerialization.DeserializeCefValue(cefValue.Object);
            CollectionAssert.AreEqual(returnValue, ret);
        }

        [Test]
        public void DeserializeCefValue_HandlesNestedDictionary()
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
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(v => v.GetValueType()).Returns(CefValueType.Dictionary);

            Mock<ICefDictionaryValue> cefOuterDictionary = new Mock<ICefDictionaryValue>();
            cefOuterDictionary.Setup(v => v.GetKeys()).Returns(returnValue.Keys.ToArray());
            foreach (string key in returnValue.Keys)
            {
                cefOuterDictionary.Setup(v => v.GetValueType(key)).Returns(CefValueType.Dictionary);
                Mock<ICefDictionaryValue> cefInnerDictionary = new Mock<ICefDictionaryValue>();
                cefInnerDictionary.Setup(v => v.GetKeys()).Returns(returnValue[key].Keys.ToArray());
                foreach (string innerkey in returnValue[key].Keys)
                {
                    cefInnerDictionary.Setup(v => v.GetValueType(innerkey)).Returns(CefValueType.Double);
                    cefInnerDictionary.Setup(v => v.GetDouble(innerkey)).Returns(returnValue[key][innerkey]);
                }
                cefOuterDictionary.Setup(v => v.GetDictionary(key)).Returns(cefInnerDictionary.Object);
            }
            cefValue.Setup(v => v.GetDictionary()).Returns(cefOuterDictionary.Object);
            var ret = (IDictionary<string, object>)CefValueSerialization.DeserializeCefValue(cefValue.Object);
            CollectionAssert.AreEqual(returnValue, ret);
        }

        [Test]
        public void DeserializeCefValue_HandlesNestedComplexDictionary()
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
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(v => v.GetValueType()).Returns(CefValueType.Dictionary);

            Mock<ICefDictionaryValue> cefOuterDictionary = new Mock<ICefDictionaryValue>();
            cefOuterDictionary.Setup(v => v.GetKeys()).Returns(returnValue.Keys.ToArray());
            foreach (string key in returnValue.Keys)
            {
                cefOuterDictionary.Setup(v => v.GetValueType(key)).Returns(CefValueType.Dictionary);
                Mock<ICefDictionaryValue> cefInnerDictionary = new Mock<ICefDictionaryValue>();
                cefInnerDictionary.Setup(v => v.GetKeys()).Returns(returnValue[key].Keys.ToArray());
                foreach (string innerkey in returnValue[key].Keys)
                {
                    Mock<ICefListValue> cefList = new Mock<ICefListValue>();
                    cefList.Setup(v => v.Count).Returns(returnValue[key][innerkey].Count);
                    for (int i = 0; i < returnValue[key][innerkey].Count; i++)
                    {
                        cefList.Setup(v => v.GetValueType(i)).Returns(CefValueType.Double);
                        cefList.Setup(v => v.GetDouble(i)).Returns(returnValue[key][innerkey][i]);
                    }
                    cefInnerDictionary.Setup(v => v.GetValueType(innerkey)).Returns(CefValueType.List);
                    cefInnerDictionary.Setup(v => v.GetList(innerkey)).Returns(cefList.Object);
                }
                cefOuterDictionary.Setup(v => v.GetDictionary(key)).Returns(cefInnerDictionary.Object);
            }
            cefValue.Setup(v => v.GetDictionary()).Returns(cefOuterDictionary.Object);
            var ret = (IDictionary<string, object>)CefValueSerialization.DeserializeCefValue(cefValue.Object);
            CollectionAssert.AreEqual(returnValue, ret);
        }

        [Test]
        public void DeserializeCefValue_HandlesSimpleList()
        {
            var returnValue = new List<int>() { 1, 2, 3 };
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(v => v.GetValueType()).Returns(CefValueType.List);

            Mock<ICefListValue> cefList = new Mock<ICefListValue>();
            cefList.Setup(v => v.Count).Returns(returnValue.Count);
            for(int i=0; i < returnValue.Count; i++)
            {
                cefList.Setup(v => v.GetValueType(i)).Returns(CefValueType.Int);
                cefList.Setup(v => v.GetInt(i)).Returns(returnValue[i]);
            }
            cefValue.Setup(v => v.GetList()).Returns(cefList.Object);
            var ret = (IList<object>)CefValueSerialization.DeserializeCefValue(cefValue.Object);
            CollectionAssert.AreEqual(returnValue, ret);
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

            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(v => v.GetValueType()).Returns(CefValueType.List);

            Mock<ICefListValue> cefOuterList = new Mock<ICefListValue>();
            cefOuterList.Setup(v => v.Count).Returns(returnValue.Count);
            for (int i = 0; i < returnValue.Count; i++)
            {
                cefOuterList.Setup(v => v.GetValueType(i)).Returns(CefValueType.List);
                Mock<ICefListValue> cefInnerList = new Mock<ICefListValue>();
                cefInnerList.Setup(v => v.Count).Returns(returnValue[i].Count);
                for (int j = 0; j < returnValue[i].Count; j++)
                {
                    cefInnerList.Setup(v => v.GetValueType(j)).Returns(CefValueType.Double);
                    cefInnerList.Setup(v => v.GetDouble(j)).Returns(returnValue[i][j]);
                }
                cefOuterList.Setup(v => v.GetList(i)).Returns(cefInnerList.Object);
            }
            cefValue.Setup(v => v.GetList()).Returns(cefOuterList.Object);
            var ret = (IList<object>)CefValueSerialization.DeserializeCefValue(cefValue.Object);
            CollectionAssert.AreEqual(returnValue, ret);
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

            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(v => v.GetValueType()).Returns(CefValueType.List);

            Mock<ICefListValue> cefOuterList = new Mock<ICefListValue>();
            cefOuterList.Setup(v => v.Count).Returns(returnValue.Count);
            for (int i = 0; i < returnValue.Count; i++)
            {
                cefOuterList.Setup(v => v.GetValueType(i)).Returns(CefValueType.List);
                Mock<ICefListValue> cefInnerList = new Mock<ICefListValue>();
                cefInnerList.Setup(v => v.Count).Returns(returnValue[i].Count);
                for (int j = 0; j < returnValue[i].Count; j++)
                {
                    Mock<ICefDictionaryValue> cefDictionary = new Mock<ICefDictionaryValue>();
                    cefDictionary.Setup(v => v.GetKeys()).Returns(returnValue[i][j].Keys.ToArray());
                    foreach (string key in returnValue[i][j].Keys)
                    {
                        cefDictionary.Setup(v => v.GetValueType(key)).Returns(CefValueType.String);
                        cefDictionary.Setup(v => v.GetString(key)).Returns(returnValue[i][j][key]);
                    }
                    cefInnerList.Setup(v => v.GetValueType(j)).Returns(CefValueType.Dictionary);
                    cefInnerList.Setup(v => v.GetDictionary(j)).Returns(cefDictionary.Object);
                }
                cefOuterList.Setup(v => v.GetList(i)).Returns(cefInnerList.Object);
            }
            cefValue.Setup(v => v.GetList()).Returns(cefOuterList.Object);
            var ret = (IList<object>)CefValueSerialization.DeserializeCefValue(cefValue.Object);
            CollectionAssert.AreEqual(returnValue, ret);
        }

        #endregion

        #region FromCefBinary
        [Test]
        public void FromCefBinary_HandlesEmptyByteArray()
        {
            var returnValue = new byte[0];
            Mock<ICefBinaryValue> byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns(returnValue);
            Assert.AreEqual(returnValue, CefValueSerialization.FromCefBinary(byteArray.Object, out var kind));
            Assert.AreEqual(BinaryMagicBytes.Binary, kind);
        }

        [Test]
        public void FromCefBinary_HandlesByteArray()
        {
            var returnValue = new byte[] { 100, 150, 254 };
            Mock<ICefBinaryValue> byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns((new byte[] { 1 }.Concat(returnValue)).ToArray());
            Assert.AreEqual(returnValue, CefValueSerialization.FromCefBinary(byteArray.Object, out var kind));
            Assert.AreEqual(BinaryMagicBytes.Binary, kind);
        }

        [Test]
        public void FromCefBinary_ThrowsExceptionOnUnknownByteType()
        {
            var returnValue = new byte[] { 10, 100, 150, 254 };
            Mock<ICefBinaryValue> byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns(returnValue);
            Assert.Throws<InvalidOperationException>(() => CefValueSerialization.FromCefBinary(byteArray.Object, out var kind));
        }

        [Test]
        public void FromCefBinary_HandlesDateTime()
        {
            var dateTime = DateTime.Now;
            var returnValue = BitConverter.GetBytes(dateTime.Ticks);
            Mock<ICefBinaryValue> byteArray = new Mock<ICefBinaryValue>();
            byteArray.Setup(c => c.ToArray()).Returns((new byte[] { 0 }.Concat(returnValue)).ToArray());
            Assert.AreEqual(dateTime, CefValueSerialization.FromCefBinary(byteArray.Object, out var kind));
            Assert.AreEqual(BinaryMagicBytes.DateTime, kind);
        }
        #endregion
    }

    [TestFixture]
    public class SerializationTests
    {
        #region Serialize
        [Test]
        public void Serialize_HandlesNullObject()
        {
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            Serialize(null, cefValue.Object);
            cefValue.Verify(v => v.SetNull(), Times.Once());
        }

        private void AssertSerialization<T1, T2>(T1 value, Expression<Action<CefValueWrapper>> setValue, Func<T1, T2> convertType = null)
        {
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            if(convertType == null)
            {
                cefValue.Setup(setValue).Callback<T2>((data) => Assert.AreEqual(value, data));
            } else
            {
                cefValue.Setup(setValue).Callback<T2>((data) => Assert.AreEqual(convertType(value), data));
            }
            Serialize(value, cefValue.Object);
            cefValue.Verify(setValue, Times.Once());
        }

        [Test]
        public void Serialize_HandlesBooleans()
        {
            AssertSerialization<bool, bool>(true, v => v.SetBool(It.IsAny<bool>()));
        }

        [Test]
        public void Serialize_HandlesSignedIntegers16()
        {
            AssertSerialization<short, int>(Int16.MaxValue, v => v.SetInt(It.IsAny<int>()));
        }

        [Test]
        public void Serialize_HandlesSignedIntegers32()
        {
            AssertSerialization<int, int>(Int32.MaxValue, v => v.SetInt(It.IsAny<int>()));
        }

        [Test]
        public void Serialize_HandlesSignedIntegers64()
        {
            AssertSerialization<long, double>(Int64.MaxValue, v => v.SetDouble(It.IsAny<double>()));
        }

        [Test]
        public void Serialize_HandlesUnsignedIntegers16()
        {
            AssertSerialization<ushort, int>(UInt16.MinValue, v => v.SetInt(It.IsAny<int>()));
        }

        [Test]
        public void Serialize_HandlesUnsignedIntegers32()
        {
            AssertSerialization<uint, int>(UInt32.MinValue, v => v.SetInt(It.IsAny<int>()));
        }

        [Test]
        public void Serialize_HandlesUnsignedIntegers64()
        {
            AssertSerialization<ulong, double>(UInt64.MinValue, v => v.SetDouble(It.IsAny<double>()));
        }

        [Test]
        public void Serialize_HandlesBytes()
        {
            AssertSerialization<byte, int>((byte)12, v => v.SetInt(It.IsAny<int>()));
        }

        [Test]
        public void Serialize_HandlesStrings()
        {
            AssertSerialization<string,string>("this is a string", v => v.SetString(It.IsAny<string>()));
        }

        [Test]
        public void Serialize_HandlesChars()
        {
            AssertSerialization<char, string>('c', v => v.SetString(It.IsAny<string>()), (char expectedValue) => expectedValue.ToString());
        }

        [Test]
        public void Serialize_HandlesDoubles()
        {
            AssertSerialization<double, double>(10.5d, v => v.SetDouble(It.IsAny<double>()));
        }

        [Test]
        public void Serialize_HandlesFloats()
        {
            AssertSerialization<float, double>(10.5f, v => v.SetDouble(It.IsAny<double>()));
        }

        [Test]
        public void Serialize_HandlesDecimals()
        {
            AssertSerialization<decimal, double>(10.5m, v => v.SetDouble(It.IsAny<double>()));
        }

        [Test]
        public void Serialize_HandlesBinaries()
        {
            Mock<CefValueWrapper> cefValue = new Mock<CefValueWrapper>();
            var expectedValue = new byte[] { 100, 12, 254 };
            Mock<IValueProxy> valueProxy = new Mock<IValueProxy>();
            valueProxy.Setup(v => v.CreateBinary(It.IsAny<byte[]>())).Callback<byte[]>((data) => CollectionAssert.AreEqual(new byte[] { (byte)BinaryMagicBytes.Binary }.Concat(expectedValue), data));
            ValueServices.ValueProxy = valueProxy.Object;
            Serialize(expectedValue, cefValue.Object);
            valueProxy.Verify(v => v.CreateBinary(It.IsAny<byte[]>()), Times.Once());
            ValueServices.Reset();
        }
        #endregion
    }
}
