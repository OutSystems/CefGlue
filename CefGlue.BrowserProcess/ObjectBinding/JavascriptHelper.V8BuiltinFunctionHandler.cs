using System.Threading.Tasks;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    partial class JavascriptHelper
    {
        internal delegate Task<bool> ObjectBoundQueryDelegate(string objectName);

        private class V8BuiltinFunctionHandler : CefV8Handler
        {
            private readonly ObjectBoundQueryDelegate _handleObjectBoundQuery;

            public V8BuiltinFunctionHandler(ObjectBoundQueryDelegate handleObjectBoundQuery)
            {
                _handleObjectBoundQuery = handleObjectBoundQuery;
            }

            protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
            {
                returnValue = null;
                exception = null;


                switch (name)
                {
                    case BindNativeFunctionName:
                        if (arguments.Length == 1 && arguments[0].IsString)
                        {
                            var objectName = arguments[0].GetStringValue();
                            var resultingPromise = CreatePromise();
                            var boundQueryTask = _handleObjectBoundQuery(objectName);

                            boundQueryTask.ContinueWith(t => {
                                resultingPromise.ResolveOrReject((resolve, reject) =>
                                {
                                    if (t.IsFaulted)
                                    {
                                        reject(CefV8Value.CreateString(t.Exception.Message));
                                    }
                                    else
                                    {
                                        resolve(CefV8Value.CreateBool(t.Result));
                                    }
                                });
                            }, TaskContinuationOptions.ExecuteSynchronously);

                            returnValue = resultingPromise.Promise;
                        }
                        else
                        {
                            exception = "Expected 1 argument of string type";
                        }
                        break;

                    default:
                        exception = $"Unknown function '{name}'";
                        break;
                }
                
                return true;
            }
        }
    }
}
