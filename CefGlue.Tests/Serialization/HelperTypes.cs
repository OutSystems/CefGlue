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
            referenceField = default;
        }
        
        public string stringField;
        public int intField;
        public bool boolField;  
        public DateTime dateField;
        public byte[] binaryField;
        public double doubleField;
        public object referenceField;
    }

    public class Person
    {
        public Person() { }
        public Person(string name)
        {
            Name = name;
        }

        public string Name;
        public Person Parent;
        public Person Child;
    }

    public struct StructObject
    {
        public StructObject(string name, int number)
        {
            NameProp = name;
            numberField = number;
        }

        public int numberField;
        public string NameProp { get; set; }
    }

    public class CyclicObj
    {
        public string stringField;
        public CyclicObj otherObj;
    }
}