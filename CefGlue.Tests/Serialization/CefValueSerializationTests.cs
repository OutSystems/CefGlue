using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xilium.CefGlue.Common.Shared.Serialization;
using static Xilium.CefGlue.Common.Shared.Serialization.CefValueSerialization;
using static Xilium.CefGlue.Common.Shared.Serialization.DataMarkers;

namespace CefGlue.Tests.Serialization
{
    [TestFixture]
    public class SerializationTests
    {
        #region Serialize

        private void AssertSerialization<T1, T2>(T1 value, Expression<Action<CefValueWrapper>> setValue, Func<T1, T2> convertType = null)
        {
            var cefValue = new Mock<CefValueWrapper>();
            cefValue.Setup(setValue).Callback<T2>((data) => Assert.AreEqual(convertType == null ? value : convertType.Invoke(value), data));
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
            AssertSerialization<string, string>("this is a string", v => v.SetString(It.IsAny<string>()), input => $"\"{StringMarker + input}\"");
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
            var byteArray = new byte[] { 0, 1, 2, 3 };
            const string ExpectedJson = "AAECAw==";
            AssertSerialization(byteArray, v => v.SetString(It.IsAny<string>()), input => $"\"{BinaryMarker + ExpectedJson}\"");
        }

        [Test]
        public void Serialize_HandlesDateTimes()
        {
            var date = new DateTime(2000, 1, 31, 15, 00, 10);
            const string ExpectedJson = "2000-01-31T15:00:10";
            AssertSerialization(date, v => v.SetString(It.IsAny<string>()), input => $"\"{DateTimeMarker + ExpectedJson}\"");
        }

        [Test]
        public void Serialize_HandlesCyclicDictionaryReferencesWithException()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("first", dict);
            var cefValue = new Mock<CefValueWrapper>();
            Assert.Throws<InvalidOperationException>(() => Serialize(dict, cefValue.Object));
        }

        [Test]
        public void Serialize_HandlesCyclicListReferencesWithException()
        {
            var list = new List<object>();
            list.Add(list);
            var cefValue = new Mock<CefValueWrapper>();
            Assert.Throws<InvalidOperationException>(() => Serialize(list, cefValue.Object));
        }

        [Test]
        public void Serialize_HandlesSimpleDictionaries()
        {
            var dict = new Dictionary<string, int>()
            {
                { "first", 1 },
                { "second", 2 },
                { "third", 3 }
            };
            const string ExpectedJson = "{\"first\":1,\"second\":2,\"third\":3}";
            AssertSerialization(dict, v => v.SetString(It.IsAny<string>()), input => ExpectedJson);
        }

        [Test]
        public void Serialize_HandlesNestedDictionaries()
        {
            var dict = new Dictionary<string, Dictionary<string, double>>()
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
            const string ExpectedJson = "{\"first\":{\"first_first\":1,\"first_second\":2,\"first_third\":3},\"second\":{\"second_first\":4,\"second_second\":5,\"second_third\":6},\"third\":{\"third_first\":7,\"third_second\":8,\"third_third\":9}}";
            AssertSerialization(dict, v => v.SetString(It.IsAny<string>()), input => ExpectedJson);
        }
        
        [Test]
        public void Serialize_HandlesObjects()
        {
            var obj = new ParentObj()
            {
                stringField = "parent",
                childObj = new ChildObj()
                {
                    stringField = "child",
                    intField = 1,
                    boolField = true
                }
            };
            const string ExpectedJson = "{\"stringField\":\"S#parent\",\"childObj\":{\"stringField\":\"S#child\",\"intField\":1,\"boolField\":true}}";
            AssertSerialization(obj, v => v.SetString(It.IsAny<string>()), input => ExpectedJson);
        }

        #endregion
    }
}
