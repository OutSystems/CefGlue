using System;
using System.Threading.Tasks;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{

    internal sealed class ActionTask : CefTask
    {
        private Action _action;

        public ActionTask(Action action)
        {
            _action = action;
        }

        protected override void Execute()
        {
            _action();
            _action = null;
        }

        public static void Run(Action action, CefThreadId threadId = CefThreadId.UI)
        {
            CefRuntime.PostTask(threadId, new ActionTask(action));
        }
    }

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
                void Log(string log)
                {
                    var processId = System.Diagnostics.Process.GetCurrentProcess().Id;
                    System.IO.File.AppendAllText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "cefglue" + processId + ".txt"), log + "\n");
                }

                returnValue = null;
                exception = null;
                
                switch (name)
                {
                    case BindNativeFunctionName:
                        {
                            var (valid, argException) = CheckArguments(arguments, a => a.IsString);
                            if (valid)
                            {
                                Task<bool> boundQueryTask;
                                PromiseHolder resultingPromise;

                                var objectName = arguments[0].GetStringValue();

                                Log("Calling bind of " + objectName);

                                var v8Context = CefV8Context.GetCurrentContext();
                                using (var context = v8Context.EnterOrFail(shallDispose: false)) // context will be released when promise is resolved
                                {
                                    resultingPromise = context.V8Context.CreatePromise();
                                    returnValue = resultingPromise.Promise; // do not dispose, because it will be delivered to cef

                                    boundQueryTask = _nativeObjectRegistry.Bind(objectName);
                                }

                                
                                boundQueryTask.ContinueWith(t =>
                                {
                                    
                                    Log("1 Continuing bind of " + objectName + " / context valid:" + resultingPromise.Context.IsValid);
                                    v8Context.GetTaskRunner().PostTask(new ActionTask(() =>
                                    {
                                        Log("2 Continuing bind of " + objectName + " / context valid:" + resultingPromise.Context.IsValid);

                                        using (CefObjectTracker.StartTracking())
                                        using (resultingPromise.Context.EnterOrFail())
                                        {
                                            resultingPromise.ResolveOrReject((resolve, reject) =>
                                            {
                                                if (t.IsFaulted)
                                                {
                                                    var exceptionMsg = CefV8Value.CreateString(t.Exception.Message);
                                                    reject(exceptionMsg);
                                                }
                                                else
                                                {
                                                    var result = CefV8Value.CreateBool(t.Result);
                                                    resolve(result);
                                                }
                                            });
                                        }
                                    }));
                                }, TaskContinuationOptions.ExecuteSynchronously);
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

            private static (bool, string) CheckArguments(CefV8Value[] arguments, params Func<CefV8Value, bool>[] argsValidators)
            {
                if (arguments.Length != argsValidators.Length)
                {
                    return (false, $"Expected {argsValidators.Length} arguments but got {arguments.Length}");
                }

                for (var i = 0; i < arguments.Length; i++)
                {
                    if (!argsValidators[i](arguments[i]))
                    {
                        return (false, $"Unexpected argument type in position {i + 1}");
                    }
                }

                return (true, "");
            }
        }
    }
}
