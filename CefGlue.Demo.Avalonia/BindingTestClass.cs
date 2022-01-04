using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Demo.Avalonia
{
    internal class BindingTestClass
    {
        private static readonly InnerObject[] items;

        static BindingTestClass()
        {
            items = new InnerObject[10000];
            for (int i = 0; i < 10000; i++)
            {
                items[i] = new InnerObject()
                {
                    Name = "random string " + i, Value = i,
                    Name1 = "random string " + i, Value1 = i,
                    Name2 = "random string " + i, Value2 = i,
                    Name3 = "random string " + i, Value3 = i,
                    Name4 = "random string " + i, Value4 = i,
                    Name5 = "random string " + i, Value5 = i,
                    Name6 = "random string " + i, Value6 = i,
                    Name7 = "random string " + i, Value7 = i,
                    Name8 = "random string " + i, Value8 = i,
                    Name9 = "random string " + i, Value9 = i,
                };
            }
        }
        
        public class InnerObject
        {
            public string Name;
            public int Value;
            public string Name1;
            public int Value1;
            public string Name2;
            public int Value2;
            public string Name3;
            public int Value3;
            public string Name4;
            public int Value4;
            public string Name5;
            public int Value5;
            public string Name6;
            public int Value6;
            public string Name7;
            public int Value7;
            public string Name8;
            public int Value8;
            public string Name9;
            public int Value9;
        }

        public DateTime GetDate()
        {
            return DateTime.Now;
        }

        public string GetString()
        {
            return "Hello World!";
        }

        public int GetInt()
        {
            return 10;
        }

        public double GetDouble()
        {
            return 10.45;
        }

        public bool GetBool()
        {
            return true;
        }

        public InnerObject[] GetObjects()
        {
            return items;
        }
        
        public string[] GetList()
        {
            return new[] { "item 1", "item 2", "item 3" };
        }

        public IDictionary<string, object> GetDictionary()
        {
            return new Dictionary<string, object>
            {
                { "Name", "This is a dictionary" },
                { "Value", 10.5 }
            };
        }

        public object GetObject()
        {
            return new InnerObject { Name = "This is an object", Value = 5 };
        }

        public object GetObjectWithParams(int anIntParam, string aStringParam, InnerObject anObjectParam, int[] intArrayParam)
        {
            return new InnerObject { Name = "This is an object", Value = 5 };
        }
        
        public async Task<bool> AsyncGetObjectWithParams(string aStringParam)
        {
            Console.WriteLine(DateTime.Now + ": Called " + nameof(AsyncGetObjectWithParams));
            await Task.Delay(5000).ConfigureAwait(false);
            Console.WriteLine(DateTime.Now + ":  Continuing " + nameof(AsyncGetObjectWithParams));
            return true;
        }

        public string[] GetObjectWithParamArray(int anIntParam, params string[] paramWithParamArray)
        {
            return paramWithParamArray;
        }
    }
}
