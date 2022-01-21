using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Demo.Avalonia
{
    internal class BindingTestClass
    {
        public class InnerObject
        {
            public string Name;
            public int Value;
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
