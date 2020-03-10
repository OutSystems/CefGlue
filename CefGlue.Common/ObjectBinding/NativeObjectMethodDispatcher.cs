using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;
using Xilium.CefGlue.Common.Serialization;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectMethodDispatcher : IDisposable
    {
        private class MethodExecutionContext
        {
            public readonly int CallId;
            public readonly string ObjectName;
            public readonly string MemberName;
            public readonly object[] Arguments;
            public readonly CefFrame Frame;

            public MethodExecutionContext(int callId, string objectName, string memberName, object[] arguments, CefFrame frame)
            {
                CallId = callId;
                ObjectName = objectName;
                MemberName = memberName;
                Arguments = arguments;
                Frame = frame;
            }
        }

        private readonly NativeObjectRegistry _objectRegistry;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly BlockingCollection<MethodExecutionContext> _pendingTasks = new BlockingCollection<MethodExecutionContext>();
    
        private ManualResetEvent _dispatchExitWaitHandle;
        private bool _allowParallelDispatching;

        public NativeObjectMethodDispatcher(MessageDispatcher dispatcher, NativeObjectRegistry objectRegistry, bool allowParallelDispatching)
        {
            _objectRegistry = objectRegistry;
            _allowParallelDispatching = allowParallelDispatching;

            dispatcher.RegisterMessageHandler(Messages.NativeObjectCallRequest.Name, HandleNativeObjectCallRequest);

            Task.Factory.StartNew(DispatchNativeObjectCalls, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void HandleNativeObjectCallRequest(MessageReceivedEventArgs args)
        {
            // message and arguments must be deserialized at this point because it will be disposed after
            using (var message = Messages.NativeObjectCallRequest.FromCefMessage(args.Message))
            {
                var arguments = CefValueSerialization.DeserializeCefList<object>(message.Arguments);

                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                _pendingTasks.Add(new MethodExecutionContext(message.CallId, message.ObjectName, message.MemberName, arguments, args.Frame));
            }
        }

        private object ExecuteMethod(object targetObj, MethodInfo method, object[] args, JavascriptObjectMethodCallHandler methodHandler)
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
                if (optionalArgs.Any())
                {
                    convertedArgs.Add(JavascriptToNativeTypeConverter.ConvertToNative(optionalArgs, optionalParam.ParameterType));
                }
            }

            return convertedArgs.ToArray();
        }

        private static bool IsParamArray(ParameterInfo paramInfo)
        {
            return paramInfo.GetCustomAttribute(typeof(ParamArrayAttribute), false) != null;
        }

        private void DispatchNativeObjectCalls()
        {
            _dispatchExitWaitHandle = new ManualResetEvent(false);

            var dispatch =
                _allowParallelDispatching ?
                    new Action<MethodExecutionContext>(context => Task.Run(() => DispatchNativeObjectCall(context))) :
                    DispatchNativeObjectCall;

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var context = _pendingTasks.Take(_cancellationTokenSource.Token);
                    if (context != null)
                    {
                        dispatch(context);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // stop
            }
            finally
            {
                _dispatchExitWaitHandle.Set();
            }
        }

        private void DispatchNativeObjectCall(MethodExecutionContext context) 
        { 
            var resultMessage = new Messages.NativeObjectCallResult()
            {
                CallId = context.CallId,
                Result = new CefValueHolder(),
            };

            try
            {
                var targetObj = _objectRegistry.Get(context.ObjectName);
                if (targetObj != null)
                {
                    var nativeMethod = targetObj.GetNativeMethod(context.MemberName);
                    if (nativeMethod != null)
                    {
                        var result = ExecuteMethod(targetObj.Target, nativeMethod, context.Arguments, targetObj.MethodHandler);
                        CefValueSerialization.Serialize(result, resultMessage.Result);
                        resultMessage.Success = true;
                    }
                    else
                    {
                        resultMessage.Success = false;
                        resultMessage.Exception = $"Object does not have a {context.ObjectName} method.";
                    }
                }
                else
                {
                    resultMessage.Success = false;
                    resultMessage.Exception = $"Object named {context.ObjectName} was not found. Make sure it was registered before.";
                }
            }
            catch (Exception e)
            {
                resultMessage.Success = false;
                resultMessage.Exception = e.Message;
            }

            using (var cefMessage = resultMessage.ToCefProcessMessage())
            {
                if (context.Frame.IsValid)
                {
                    context.Frame.SendProcessMessage(CefProcessId.Renderer, cefMessage);
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _dispatchExitWaitHandle?.WaitOne();
            _cancellationTokenSource.Dispose();
        }
    }
}
