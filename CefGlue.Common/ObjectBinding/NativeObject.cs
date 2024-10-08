using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObject
    {
        private readonly object _target;
        private readonly IDictionary<string, NativeMethod> _methods;
        private readonly object _methodHandlerTarget;
        private readonly NativeMethod _methodHandler;

        public Messaging Messaging { get; }

        public NativeObject(Messaging messaging, string name, object target, MethodCallHandler methodHandler = null)
        {
            Messaging = messaging;
            Name = name;
            _target = target;
            _methods = GetObjectMembers(target);

            if (methodHandler != null)
            {
                _methodHandler = new NativeMethod(this, methodHandler.Method);
                _methodHandlerTarget = methodHandler.Target;
            }
        }

        public string Name { get; }

        public IEnumerable<NativeMethod> Methods => _methods.Values;

        public void ExecuteMethod(string methodName, byte[] serializedArguments, Action<object, Exception> handleResult)
        {
            if (!_methods.TryGetValue(methodName ?? "", out var method))
            {
                handleResult(default, new Exception($"Object does not have a {methodName} method."));
                return;
            }

            var arguments = method.ParameterTypes.Length > 0 
                ? Messaging.Deserialize(serializedArguments, method.ParameterTypes)
                : [];
            ExecuteMethod(methodName, arguments, handleResult);
        }

        public void ExecuteMethod(string methodName, object[] args, Action<object, Exception> handleResult)
        {
            if (!_methods.TryGetValue(methodName ?? "", out var method))
            {
                handleResult(default, new Exception($"Object does not have a {methodName} method."));
                return;
            }

            if (_methodHandler == null)
            {
                method.Execute(_target, args, handleResult);
                return;
            }

            var innerMethod = method.MakeDelegate(_target, args);
            _methodHandler.Execute(_methodHandlerTarget, innerMethod, (result, exception) =>
            {
                if (result is Task task)
                {
                    task.ContinueWith(t =>
                    {
                        var taskResult = GenericTaskAwaiter.GetResultFrom(t);
                        handleResult(taskResult.Result, taskResult.Exception);
                    });
                    return;
                }

                handleResult(result, exception);
            });
        }

        private IDictionary<string, NativeMethod> GetObjectMembers(object obj)
        {
            var methods = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => !m.IsSpecialName);
            return methods.ToDictionary(m => m.Name.ToCamelCase(), m => new NativeMethod(this, m));
        }
    }
}
