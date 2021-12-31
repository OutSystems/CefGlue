using System;
using System.Threading.Tasks;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectInteropAsyncMethodInterceptorTests : NativeObjectInteropTests
    {
        protected override void RegisterObject()
        {
            nativeObject = new NativeObject();
            Browser.RegisterJavascriptObject(nativeObject, ObjName, Intercept);
        }

        private Task<object> Intercept(Func<object> originalMethod)
        {
            return Task.Run(originalMethod);
        }
    }
}
