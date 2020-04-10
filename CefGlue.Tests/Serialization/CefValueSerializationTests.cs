using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Shared.Serialization;
using static Xilium.CefGlue.Common.Shared.Serialization.CefValueSerialization;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class SerializationTests
    {
        #region Serialize

        private void AssertSerialization<T1, T2>(T1 value, Expression<Action<CefValueWrapper>> setValue, Func<T1, T2> convertType = null)
        {
            var cefValue = new Mock<CefValueWrapper>();
            if (convertType == null)
            {
                cefValue.Setup(setValue).Callback<T2>((data) => Assert.AreEqual(value, data));
            }
            else
            {
                cefValue.Setup(setValue).Callback<T2>((data) => Assert.AreEqual(convertType(value), data));
            }
            Serialize(value, cefValue.Object);
            cefValue.Verify(setValue, Times.Once());
        }

        [Test]
        public void Serialize_HandlesNullObject()
        {
            var cefValue = new Mock<CefValueWrapper>();
            Serialize(null, cefValue.Object);
            cefValue.Verify(v => v.SetNull(), Times.Once());
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
            AssertSerialization<string, string>("this is a string", v => v.SetString(It.IsAny<string>()));
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
            var cefValue = new Mock<CefValueWrapper>();
            var expectedValue = new byte[] { 100, 12, 254 };
            var valueProxy = new Mock<IValueProvider>();
            valueProxy.Setup(v => v.CreateBinary(It.IsAny<byte[]>())).Callback<byte[]>((data) => CollectionAssert.AreEqual(new byte[] { (byte)BinaryMagicBytes.Binary }.Concat(expectedValue), data));
            ValueServices.ValueProxy = valueProxy.Object;
            Serialize(expectedValue, cefValue.Object);
            valueProxy.Verify(v => v.CreateBinary(It.IsAny<byte[]>()), Times.Once());
            cefValue.Verify(v => v.SetBinary(It.IsAny<ICefBinaryValue>()), Times.Once());
        }

        [Test]
        public void Serialize_HandlesDateTimes()
        {
            var cefValue = new Mock<CefValueWrapper>();
            var dateTime = DateTime.Now;
            var expectedValue = BitConverter.GetBytes(dateTime.Ticks);
            var valueProxy = new Mock<IValueProvider>();
            valueProxy.Setup(v => v.CreateBinary(It.IsAny<byte[]>())).Callback<byte[]>((data) => CollectionAssert.AreEqual(new byte[] { (byte)BinaryMagicBytes.DateTime }.Concat(expectedValue), data));
            ValueServices.ValueProxy = valueProxy.Object;
            Serialize(dateTime, cefValue.Object);
            valueProxy.Verify(v => v.CreateBinary(It.IsAny<byte[]>()), Times.Once());
            cefValue.Verify(v => v.SetBinary(It.IsAny<ICefBinaryValue>()), Times.Once());
        }

        [Test]
        public void Serialize_HandlesCyclicDictionaryReferencesWithException()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("first", dict);
            var cefValue = new Mock<CefValueWrapper>();
            var valueProxy = new Mock<IValueProvider>();
            ValueServices.ValueProxy = valueProxy.Object;
            Assert.Throws<InvalidOperationException>(() => Serialize(dict, cefValue.Object));
        }

        [Test]
        public void Serialize_HandlesCyclicListReferencesWithException()
        {
            var list = new List<object>();
            list.Add(list);
            var cefValue = new Mock<CefValueWrapper>();
            var valueProxy = new Mock<IValueProvider>();
            ValueServices.ValueProxy = valueProxy.Object;
            Assert.Throws<InvalidOperationException>(() => Serialize(list, cefValue.Object));
        }

        [Test]
        public void Serialize_HandlesSimpleDictionaries()
        {
            var returnValue = new Dictionary<string, int>()
            {
                { "first", 1 },
                { "second", 2 },
                { "third", 3 }
            };
            var compareValue = new Dictionary<string, int>();
            var cefValue = new Mock<CefValueWrapper>();
            var valueProxy = new Mock<IValueProvider>();
            var dictionaryValue = new Mock<ICefDictionaryValue>();
            dictionaryValue.Setup(v => v.SetInt(It.IsAny<string>(), It.IsAny<int>())).Callback<string, int>((key, value) => compareValue[key] = value);
            valueProxy.Setup(v => v.CreateDictionary()).Returns(dictionaryValue.Object);
            ValueServices.ValueProxy = valueProxy.Object;
            cefValue.Setup(v => v.SetDictionary(It.IsAny<ICefDictionaryValue>())).Callback<ICefDictionaryValue>((data) => Assert.AreSame(dictionaryValue.Object, data));
            Serialize(returnValue, cefValue.Object);
            cefValue.Verify(v => v.SetDictionary(It.IsAny<ICefDictionaryValue>()), Times.Once());
            CollectionAssert.AreEqual(returnValue, compareValue);
        }

        [Test]
        public void Serialize_HandlesNestedDictionaries()
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
            var cefValue = new Mock<CefValueWrapper>();
            var valueProxy = new Mock<IValueProvider>();

            var outerDictionary = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, double> innerDictionary = null;

            int currentIndex = 0;
            var outerDictionaryValue = new Mock<ICefDictionaryValue>();
            outerDictionaryValue.Setup(v => v.SetDictionary(It.IsAny<string>(), It.IsAny<ICefDictionaryValue>())).Callback<string, ICefDictionaryValue>((key, value) => {
                currentIndex++;
                outerDictionary[key] = innerDictionary;
            });

            var innerDictionaryValues = new Mock<ICefDictionaryValue>[returnValue.Count];
            var innerDictionaries = new Dictionary<string, double>[returnValue.Count];

            for (int iter = 0; iter < returnValue.Count(); iter++)
            {
                if (innerDictionaryValues[iter] == null)
                {
                    innerDictionaryValues[iter] = new Mock<ICefDictionaryValue>();
                }
                innerDictionaryValues[iter].Setup(v => v.SetDouble(It.IsAny<string>(), It.IsAny<double>())).Callback<string, double>((key, value) =>
                {
                    if (innerDictionaries[currentIndex] == null)
                    {
                        innerDictionaries[currentIndex] = new Dictionary<string, double>();
                    }
                    innerDictionary = innerDictionaries[currentIndex];
                    innerDictionaries[currentIndex][key] = value;
                });
            }

            var sequence = valueProxy.SetupSequence(v => v.CreateDictionary()).Returns(outerDictionaryValue.Object);
            for (int i = 0; i < returnValue.Count; i++)
            {
                sequence.Returns(innerDictionaryValues[i].Object);
            }

            ValueServices.ValueProxy = valueProxy.Object;
            cefValue.Setup(v => v.SetDictionary(It.IsAny<ICefDictionaryValue>())).Callback<ICefDictionaryValue>((data) => Assert.AreSame(outerDictionaryValue.Object, data));
            Serialize(returnValue, cefValue.Object);
            cefValue.Verify(v => v.SetDictionary(It.IsAny<ICefDictionaryValue>()), Times.Once());
            CollectionAssert.AreEqual(returnValue, outerDictionary);
        }

        [OneTimeTearDown]
        public void ClassTearDown()
        {
            ValueServices.Reset();
        }

        #endregion
    }
}
