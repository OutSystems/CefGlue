using System;
using System.Threading.Tasks;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    partial class JavascriptHelper
    {
        private class V8BuiltinFunctionHandler : CefV8Handler
        {
            private readonly INativeObjectRegistry _nativeObjectRegistry;

            public V8BuiltinFunctionHandler(INativeObjectRegistry nativeObjectRegistry)
            {
                _nativeObjectRegistry = nativeObjectRegistry;
            }

            protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
            {
                returnValue = null;
                exception = null;

                switch (name)
                {
                    case BindNativeFunctionName:
                        {
                            var (valid, argException) = CheckArguments(arguments, a => a.IsString);
                            if (valid)
                            {
                                var objectName = arguments[0].GetStringValue();
                                var resultingPromise = CreatePromise();
                                var boundQueryTask = _nativeObjectRegistry.Bind(objectName);

                                boundQueryTask.ContinueWith(t =>
                                {
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
                                exception = argException;
                            }

                            break;
                        }

                    case UnbindNativeFunctionName:
                        {
                            var (valid, argException) = CheckArguments(arguments, a => a.IsString);
                            if (valid)
                            {
                                var objectName = arguments[0].GetStringValue();
                                _nativeObjectRegistry.Unbind(objectName);
                            }

                            break;
                        }

                    default:

                        exception = $"Unknown function '{name}'";
                        break;
                }
                
                return true;
            }
        }

        private static (bool, string) CheckArguments(CefV8Value[] arguments, params Func<CefV8Value, bool>[] argsValidators)
        {
            if (arguments.Length != argsValidators.Length)
            {
                return (false, $"Expected {argsValidators.Length} arguments but got {arguments.Length}");
            }

            for(var i = 0; i < arguments.Length; i++)
            {
                if (!argsValidators[i](arguments[i]))
                {
                    return (false, $"Unexpected argument type in position {i+1}");
                }
            }

            return (true, "");
        }
    }
}
