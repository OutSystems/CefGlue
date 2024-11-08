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
        private readonly NativeObject _nativeObject;
        private readonly MethodInfo _methodInfo;

        public Type[] ParameterTypes { get; }

        public int MandatoryParameterCount { get; }

        public bool HasOptionalParameters { get; }

        public string Name => _methodInfo.Name;

        public NativeMethod(NativeObject nativeObject, MethodInfo methodInfo)
        {
            _nativeObject = nativeObject;
            _methodInfo = methodInfo;

            ParameterInfo[] parameters = methodInfo.GetParameters();
            HasOptionalParameters = IsVariableParameterList(parameters);
            MandatoryParameterCount = HasOptionalParameters
                ? parameters.Length - 1
                : parameters.Length;
            this.ParameterTypes = parameters
                .Select(p => p.ParameterType)
                .ToArray();
        }

        public Func<object> MakeDelegate(object targetObj, object[] parameters)
        {
            return () => ExecuteMethod(targetObj, parameters);
        }

        public void Execute(object targetObj, Func<object> innerMethod, Action<object, Exception> handleResult)
        {
            Execute(targetObj, new[] { innerMethod }, handleResult);
        }

        public void Execute(object targetObj, object[] parameters, Action<object, Exception> handleResult)
        {
            object result = null;
            Exception exception = null;
            try
            {
                result = ExecuteMethod(targetObj, parameters);
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

        private object ExecuteMethod(object targetObj, object[] parameters)
        {
            try
            {
                // TODO improve call perf
                return _methodInfo.Invoke(targetObj, parameters);
            }
            catch (TargetInvocationException e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException ?? e).Throw();
                return null;
            }
        }

        private static bool IsVariableParameterList(ParameterInfo[] paramInfo)
            => paramInfo.Length > 0
            && paramInfo[^1].IsDefined(typeof(ParamArrayAttribute), false);
    }
}