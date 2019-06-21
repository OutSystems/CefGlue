namespace Xilium.CefGlue.Demo.Avalonia
{
    internal class BindingTestClass
    {
        private class InnerObject
        {
            public string Name;
            public int Value;
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
            return new [] { "item 1", "item 2", "item 3" };
        }

        public object GetObject()
        {
            return new InnerObject { Name = "This is an object", Value = 5 };
        }
    }
}
