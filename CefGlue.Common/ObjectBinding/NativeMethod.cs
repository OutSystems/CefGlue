using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeMethod
    {
        private readonly MethodInfo _methodInfo;
        private readonly Type[] _parameterTypes;
        private readonly int _mandatoryParametersCount;
        private readonly bool _hasOptionalParameters;
        
        public NativeMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;

            var parameters = methodInfo.GetParameters();
            _hasOptionalParameters = IsParamArray(parameters.LastOrDefault());
            _mandatoryParametersCount = parameters.Length - (_hasOptionalParameters ? 1 : 0);
            var parameterTypes = parameters
                .Take(_mandatoryParametersCount)
                .Select(p => p.ParameterType);
            if (_hasOptionalParameters)
            {
                parameterTypes = parameterTypes.Append(parameters.Last().ParameterType.GetElementType());
            }
            _parameterTypes = parameterTypes.ToArray();
        }
        
        public Func<object> MakeDelegate<T>(object targetObj, T args)
        {
            var convertedArgs = ConvertArguments(args);
            return () => ExecuteMethod(targetObj, convertedArgs);
        }

        public void Execute<T>(object targetObj, T args, Action<object, Exception> handleResult)
        {
            Execute(targetObj, ConvertArguments(args), handleResult);
        }

        public void Execute(object targetObj, Func<object> innerMethod, Action<object, Exception> handleResult)
        {
            Execute(targetObj, new[] { innerMethod }, handleResult);
        }

        public void Execute(object targetObj, object[] args, Action<object, Exception> handleResult)
        {
            object result = null;
            Exception exception = null;
            try
            {
                result = ExecuteMethod(targetObj, args);
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

            // convertedArgumentsWithOptionals/exception is available
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
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
                return null;
            }
        }

        private object[] ConvertArguments<T>(T args)
        {
            var argsAsObject = (object)args;
            
            if (typeof(T) == typeof(string)) {
                return InnerConvertArguments((string)argsAsObject);
            }

            return ConvertArgumentsWithOptionals((object[])argsAsObject);
        }

        private object[] InnerConvertArguments(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                var convertedArguments = Array.Empty<object>();

                ValidateMandatoryArguments(convertedArguments);
                return convertedArguments;
            }

            var originalArguments = Deserializer.Deserialize(args, _parameterTypes);

            return ConvertArgumentsWithOptionals(originalArguments);
        }

        private object[] ConvertArgumentsWithOptionals(object[] originalArguments)
        {
            ValidateMandatoryArguments(originalArguments);

            if (!_hasOptionalParameters)
            {
                return originalArguments;
            }

            // the optionalParameterType is always a ParamArray of the last type in the ParameterTypes (eg int[])
            var convertedArgumentsWithOptionals = new object[_parameterTypes.Length];
            Array.Copy(originalArguments, convertedArgumentsWithOptionals, _mandatoryParametersCount);
            
            var optionalArgsCount = originalArguments.Length - _mandatoryParametersCount;
            var optionalParamType = _parameterTypes.Last();
            var optionalArgsArray = Array.CreateInstance(optionalParamType, optionalArgsCount);

            Array.Copy(originalArguments, _mandatoryParametersCount, optionalArgsArray, 0, optionalArgsCount);
            convertedArgumentsWithOptionals[_parameterTypes.Length - 1] = optionalArgsArray;

            return convertedArgumentsWithOptionals;
        }

        private void ValidateMandatoryArguments(object[] originalArguments)
        {
            if (originalArguments.Length < _mandatoryParametersCount)
            {
                throw new ArgumentException($"Number of original arguments provided does not match the number of {_methodInfo.Name} method required parameters.");
            }
        }

        private static bool IsParamArray(ParameterInfo paramInfo)
        {
            return paramInfo?.GetCustomAttribute(typeof(ParamArrayAttribute), false) != null;
        }
    }
}