using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal static class NativeObjectMethodExecutor
    {
        public static object ExecuteMethod(object targetObj, MethodInfo method, object[] args, JavascriptObjectMethodCallHandler methodHandler = null)
        {
            var convertedArgs = ConvertArguments(method, args);

            if (methodHandler != null)
            {
                return methodHandler(() => method.Invoke(targetObj, convertedArgs));
            }

            // TODO improve call perf
            return method.Invoke(targetObj, convertedArgs);
        }

        private static object[] ConvertArguments(MethodInfo method, object[] args)
        {
            var parameters = method.GetParameters();
            var mandatoryParams = parameters.TakeWhile(p => !IsParamArray(p)).ToArray();

            if (args.Length < mandatoryParams.Length)
            {
                throw new ArgumentException($"Number of arguments provided does not match the number of {method.Name} method required parameters.");
            }

            var argIndex = 0;
            var convertedArgs = new List<object>(mandatoryParams.Length + 1);

            for (; argIndex < mandatoryParams.Length; argIndex++)
            {
                convertedArgs.Add(JavascriptToNativeTypeConverter.ConvertToNative(args[argIndex], mandatoryParams[argIndex].ParameterType));
            }

            var optionalParam = parameters.Skip(mandatoryParams.Length).FirstOrDefault();
            if (optionalParam != null)
            {
                var optionalArgs = args.Skip(argIndex).ToArray();
                convertedArgs.Add(JavascriptToNativeTypeConverter.ConvertToNative(optionalArgs, optionalParam.ParameterType));
            }

            return convertedArgs.ToArray();
        }

        private static bool IsParamArray(ParameterInfo paramInfo)
        {
            return paramInfo.GetCustomAttribute(typeof(ParamArrayAttribute), false) != null;
        }
    }
}
