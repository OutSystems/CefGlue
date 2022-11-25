using System;

namespace CefGlue.Tests.Serialization
{
    public struct ParentObj
    {
        public string stringField;
        public ChildObj childObj;
    }

    public struct ChildObj
    {
#pragma warning disable 414
        private string privateField;
#pragma warning restore 414

        public ChildObj(string privateField = "private field content")
        {
            this.privateField = privateField;
            stringField = null;
            intField = 0;
            boolField = false;
            dateField = default;
            binaryField = default;
            doubleField = default;
        }
        
        public string stringField;
        public int intField;
        public bool boolField;
        public DateTime dateField;
        public byte[] binaryField;
        public double doubleField;
    }

    public class CyclicObj
    {
        public string stringField;
        public CyclicObj otherObj;
    }
}