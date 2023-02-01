﻿using System;
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
            return () => ExecuteMethod(targetObj, ConvertArguments(args));
        }

        public void Execute<T>(object targetObj, T args, Action<object, Exception> handleResult)
        {
            Execute(targetObj, ConvertArguments(args), handleResult);
        }

        public void Execute(object targetObj, Func<object> innerMethod, Action<object, Exception> handleResult)
        {
            Execute(targetObj, new[] { innerMethod }, handleResult);
        }

        internal void Execute(object targetObj, object[] args, Action<object, Exception> handleResult)
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
                var nativeArgs = ConvertToNative((string)argsAsObject);
                ValidateMandatoryArguments(nativeArgs);
                return nativeArgs;
            }

            var originalArgs = (object[])argsAsObject;
            
            ValidateMandatoryArguments(originalArgs);

            var argIndex = 0;
            var convertedArgs = new List<object>(_mandatoryParameters.Length + 1);

            for (; argIndex < _mandatoryParameters.Length; argIndex++)
            {
                convertedArgs.Add(originalArgs[argIndex]);
            }

            if (_optionalParameter != null)
            {
                // the optionalParameterType is always a ParamArray of some type (eg int[]), so we need the ElementType
                var optionalArgsArray = Array.CreateInstance(_optionalParameter.ParameterType.GetElementType(), originalArgs.Length - argIndex);
                var optionalArgIndex = 0;
                foreach (var optionalArg in originalArgs.Skip(argIndex))
                {
                    optionalArgsArray.SetValue(optionalArg, optionalArgIndex++);
                }
                convertedArgs.Add(optionalArgsArray);
            }

            return convertedArgs.ToArray();
        }

        private object[] ConvertToNative(string args)
        {
            return string.IsNullOrEmpty(args) ?
                Array.Empty<object>() :
                Deserializer.Deserialize<object[]>(
                    args, 
                    new Shared.Serialization.State.ParametersDeserializerState.ParametersTypes(
                        _mandatoryParameters.Select(p => p.ParameterType).ToArray(), 
                        _optionalParameter?.ParameterType));
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