using System;
using System.Threading.Tasks;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectInteropAsyncInterceptorTests : NativeObjectInteropTests
    {
        protected override void RegisterObject()
        {
            nativeObject = new NativeObject();
            Browser.RegisterJavascriptObject(nativeObject, ObjName, Intercept);
        }

        private Task<object> Intercept(Func<object> originalMethod)
        {
            var result = originalMethod.Invoke();

            // simulate an async interceptor
            return Task.Run(() => result);
        }
    }
}
