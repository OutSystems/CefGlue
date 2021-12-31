using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeMethod
    {
        private readonly MethodInfo _method;
        private readonly Func<Task, object> _asyncResultGetter;
        
        public NativeMethod(MethodInfo method, Func<Task, object> asyncResultGetter)
        {
            _method = method;
            _asyncResultGetter = asyncResultGetter;
        }

        public bool IsAsync => _asyncResultGetter != null;
        
        public object Execute(object targetObj, object[] args, JavascriptObjectMethodCallHandler methodHandler = null)
        {
            var convertedArgs = ConvertArguments(_method, args);

            if (methodHandler != null)
            {
                return methodHandler(() => _method.Invoke(targetObj, convertedArgs));
            }

            // TODO improve call perf
            return _method.Invoke(targetObj, convertedArgs);
        }

        public object GetResult(Task task)
        {
            return _asyncResultGetter(task);
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