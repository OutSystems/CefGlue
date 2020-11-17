namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal class ObjectRegistrationInfo
    {
        public ObjectRegistrationInfo(string name, string[] methodsNames)
        {
            Name = name;
            MethodsNames = methodsNames;
        }

        public string Name { get; }

        public string[] MethodsNames { get; }
    }
}
