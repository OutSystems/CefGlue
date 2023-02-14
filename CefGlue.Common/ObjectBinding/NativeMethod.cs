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
        private readonly int _mandatoryParametersLength;
        private readonly bool _hasOptionalParameters;
        
        public NativeMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;

            var parameters = methodInfo.GetParameters();
            _hasOptionalParameters = IsParamArray(parameters.LastOrDefault());
            _mandatoryParametersLength = parameters.Length - (_hasOptionalParameters ? 1 : 0);
            var parameterTypes = parameters
                .Take(_mandatoryParametersLength)
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
                var nativeArgs = InnerConvertArguments((string)argsAsObject);
                ValidateMandatoryArguments(nativeArgs);
                return nativeArgs;
            }

            var originalArgs = (object[])argsAsObject;
            
            ValidateMandatoryArguments(originalArgs);

            var convertedArgs = new object[_parameterTypes.Length];
            
            Array.Copy(originalArgs, convertedArgs, _mandatoryParametersLength);
            
            if (_hasOptionalParameters)
            {
                // the optionalParameterType is always a ParamArray of some type (eg int[]), so we need the ElementType
                var optionalArgsArray = Array.CreateInstance(_parameterTypes.Last(), originalArgs.Length - _mandatoryParametersLength);
                Array.Copy(originalArgs, _mandatoryParametersLength, optionalArgsArray, 0, optionalArgsArray.Length);
                convertedArgs[_parameterTypes.Length - 1] = optionalArgsArray;
            }

            return convertedArgs;
        }

        private object[] InnerConvertArguments(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                return Array.Empty<object>();
            }

            var convertedArguments = Deserializer.Deserialize(args, _parameterTypes);
            
            if (!_hasOptionalParameters)
            {
                return convertedArguments;
            }

            // reaching here, it means we have to convert the tempArray to a new array like
            // [arg1, ..., <optionalParameterType>[opt1, ...]]
            var convertedArgumentsWithOptionals = new object[_parameterTypes.Length];
            Array.Copy(convertedArguments, convertedArgumentsWithOptionals, _mandatoryParametersLength);
            var optionalsArray = Array.CreateInstance(_parameterTypes.Last(), convertedArguments.Length - _mandatoryParametersLength);
            Array.Copy(convertedArguments, _mandatoryParametersLength, optionalsArray, 0, optionalsArray.Length);
            convertedArgumentsWithOptionals[_parameterTypes.Length - 1] = optionalsArray;

            return convertedArgumentsWithOptionals;
        }

        private void ValidateMandatoryArguments(object[] args)
        {
            if (args.Length < _mandatoryParametersLength)
            {
                throw new ArgumentException($"Number of convertedArguments provided does not match the number of {_methodInfo.Name} method required parameters.");
            }
        }

        private static bool IsParamArray(ParameterInfo paramInfo)
        {
            return paramInfo?.GetCustomAttribute(typeof(ParamArrayAttribute), false) != null;
        }
    }
}