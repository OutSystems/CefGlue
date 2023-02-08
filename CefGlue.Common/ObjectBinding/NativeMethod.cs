using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Shared.Serialization;

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

            var argIndex = 0;
            var convertedArgsLength = _optionalParameter == null ? _mandatoryParameters.Length : _mandatoryParameters.Length + 1;
            var convertedArgs = new object[convertedArgsLength];

            for (; argIndex < _mandatoryParameters.Length; argIndex++)
            {
                convertedArgs[argIndex] = originalArgs[argIndex];
            }

            if (_optionalParameter != null)
            {
                // the optionalParameterType is always a ParamArray of some type (eg int[]), so we need the ElementType
                var optionalArgsArray = ExtractArray(originalArgs, argIndex, _optionalParameter.ParameterType.GetElementType());
                convertedArgs[convertedArgsLength - 1] = optionalArgsArray;
            }

            return convertedArgs;
        }

        private object[] InnerConvertArguments(string args)
        {
            if (string.IsNullOrEmpty(args))
            {
                return Array.Empty<object>();
            }

            var targetTypes = _mandatoryParameters.Select(p => p.ParameterType);
            Type optionalParameterElementType = null;
            
            if (_optionalParameter != null)
            {
                // the optionalParameterType is always a ParamArray of some type (eg int[]), so we need the ElementType
                optionalParameterElementType = _optionalParameter.ParameterType.GetElementType();
                targetTypes = targetTypes.Append(optionalParameterElementType);
            }

            if (targetTypes.Count() == 1)
            {
                // To force the deserializer to return an object array containing elements of the desired type,
                // more than 1 type must be passed as an argument,
                // hence, we're adding a dummy extra argument to the targetTypes
                targetTypes = targetTypes.Append(typeof(object));
            }

            var tempResult = (object[])Deserializer.Deserialize(args, targetTypes.ToArray());

            if (_optionalParameter == null)
            {
                return tempResult;
            }

            // reaching here, it means we have to convert the tempArray to a new array like
            // [arg1, ..., <optionalParameterType>[opt1, ...]]
            return 
                tempResult
                .Take(_mandatoryParameters.Length)
                .Append(ExtractArray(tempResult, _mandatoryParameters.Length, optionalParameterElementType))
                .ToArray();
        }

        private static Array ExtractArray(object[] args, int elementsToSkip, Type arrayElementType)
        {
            var result = Array.CreateInstance(arrayElementType, args.Length - elementsToSkip);
            var arrayIndex = 0;
            foreach (var arg in args.Skip(elementsToSkip))
            {
                result.SetValue(arg, arrayIndex++);
            }
            return result;
        }

        private void ValidateMandatoryArguments(object[] args)
        {
            if (args.Length < _mandatoryParameters.Length)
            {
                throw new ArgumentException($"Number of arguments provided does not match the number of {_methodInfo.Name} method required parameters.");
            }
        }

        private static bool IsParamArray(ParameterInfo paramInfo)
        {
            return paramInfo.GetCustomAttribute(typeof(ParamArrayAttribute), false) != null;
        }
    }
}