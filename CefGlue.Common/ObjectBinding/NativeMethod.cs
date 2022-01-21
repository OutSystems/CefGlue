using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeMethod
    {
        private readonly MethodInfo _methodInfo;
        private readonly ParameterInfo[] _mandatoryParameters;
        private readonly ParameterInfo _optionalParameter;

        public NativeMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;

            var parameters = methodInfo.GetParameters();
            _mandatoryParameters = parameters.TakeWhile(p => !IsParamArray(p)).ToArray();
            _optionalParameter = parameters.Skip(_mandatoryParameters.Length).FirstOrDefault();
        }
        
        public Func<object> MakeDelegate(object targetObj, object[] args)
        {
            var convertedArgs = ConvertArguments(args);

            return () => ExecuteMethod(targetObj, convertedArgs);
        }

        public void Execute(object targetObj, object[] args, Action<object, Exception> handleResult)
        {
            object result = null;
            Exception exception = null;
            try
            {
                var convertedArgs = ConvertArguments(args);
                result = ExecuteMethod(targetObj, convertedArgs);
            } 
            catch (Exception e)
            {
                exception = e;
            }

            if (result is Task task)
            {
                task.ContinueWith(t =>
                {
                    var taskResult = GenericTaskAwaiter.GetResultFrom(t);
                    handleResult(taskResult.Result, taskResult.Exception);
                });
                return;
            }

            // result/exception is available
            handleResult(result, exception);
        }

        private object ExecuteMethod(object targetObj, object[] args)
        {
            try
            {
                // TODO improve call perf
                return _methodInfo.Invoke(targetObj, args);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException ?? e;
            }
        }

        private object[] ConvertArguments(object[] args)
        {
            if (args.Length < _mandatoryParameters.Length)
            {
                throw new ArgumentException($"Number of arguments provided does not match the number of {_methodInfo.Name} method required parameters.");
            }

            var argIndex = 0;
            var convertedArgs = new List<object>(_mandatoryParameters.Length + 1);

            for (; argIndex < _mandatoryParameters.Length; argIndex++)
            {
                convertedArgs.Add(JavascriptToNativeTypeConverter.ConvertToNative(args[argIndex], _mandatoryParameters[argIndex].ParameterType));
            }

            if (_optionalParameter != null)
            {
                var optionalArgs = args.Skip(argIndex).ToArray();
                convertedArgs.Add(JavascriptToNativeTypeConverter.ConvertToNative(optionalArgs, _optionalParameter.ParameterType));
            }

            return convertedArgs.ToArray();
        }

        private static bool IsParamArray(ParameterInfo paramInfo)
        {
            return paramInfo.GetCustomAttribute(typeof(ParamArrayAttribute), false) != null;
        }
    }
}