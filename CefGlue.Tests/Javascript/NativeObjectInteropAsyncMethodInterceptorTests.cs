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
            return Task.Run(() =>
            {
                var result = originalMethod.Invoke();
                if (result is Task task)
                {
                    if (task.GetType().IsGenericType)
                    {
                        return ((dynamic) task).Result;
                    }

                    return task;
                }

                return result;
            });
        }
    }
}
