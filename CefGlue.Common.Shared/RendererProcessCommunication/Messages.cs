using System;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.Common.Shared.RendererProcessCommunication
{
    internal static class Messages
    {
        public struct JsEvaluationRequest
        {
            public const string Name = nameof(JsEvaluationRequest);

            public int TaskId;
            public string Script;
            public string Url;
            public int Line;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using (var arguments = message.Arguments)
                {
                    arguments.SetInt(0, TaskId);
                    arguments.SetString(1, Script);
                    arguments.SetString(2, Url);
                    arguments.SetInt(3, Line);
                }
                return message;
            }

            public static JsEvaluationRequest FromCefMessage(CefProcessMessage message)
            {
                using (var arguments = message.Arguments)
                {
                    return new JsEvaluationRequest()
                    {
                        TaskId = arguments.GetInt(0),
                        Script = arguments.GetString(1),
                        Url = arguments.GetString(2),
                        Line = arguments.GetInt(3)
                    };
                }
            }
        }

        public struct JsEvaluationResult
        {
            public const string Name = nameof(JsEvaluationResult);

            public int TaskId;
            public bool Success;
            public CefValueHolder Result;
            public string Exception;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using (var arguments = message.Arguments)
                {
                    arguments.SetInt(0, TaskId);
                    arguments.SetBool(1, Success);
                    Result?.AssignToListAndClearReference(arguments, 2);
                    arguments.SetString(3, Exception);
                }
                return message;
            }

            public static JsEvaluationResult FromCefMessage(CefProcessMessage message)
            {
                using (var arguments = message.Arguments)
                {
                    return new JsEvaluationResult()
                    {
                        TaskId = arguments.GetInt(0),
                        Success = arguments.GetBool(1),
                        Result = new CefValueHolder(arguments.GetValue(2)),
                        Exception = arguments.GetString(3)
                    };
                }
            }
        }

        public struct NativeObjectRegistrationRequest
        {
            public const string Name = nameof(NativeObjectRegistrationRequest);

            public string ObjectName;
            public string[] MethodsNames;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using (var arguments = message.Arguments)
                {
                    arguments.SetString(0, ObjectName);

                    using (var methods = CefListValue.Create())
                    {
                        for (var i = 0; i < MethodsNames.Length; i++)
                        {
                            methods.SetString(i, MethodsNames[i]);
                        }

                        arguments.SetList(1, methods);
                    }
                }
                return message;
            }

            public static NativeObjectRegistrationRequest FromCefMessage(CefProcessMessage message)
            {
                using (var arguments = message.Arguments)
                using (var methodsNames = arguments.GetList(1))
                {
                    return new NativeObjectRegistrationRequest()
                    {
                        ObjectName = arguments.GetString(0),
                        MethodsNames = CefValueSerialization.DeserializeCefList<string>(methodsNames)
                    };
                }
            }
        }

        public struct NativeObjectUnregistrationRequest
        {
            public const string Name = nameof(NativeObjectUnregistrationRequest);

            public string ObjectName;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using (var arguments = message.Arguments)
                {
                    arguments.SetString(0, ObjectName);
                }
                return message;
            }

            public static NativeObjectRegistrationRequest FromCefMessage(CefProcessMessage message)
            {
                using (var arguments = message.Arguments)
                {
                    return new NativeObjectRegistrationRequest()
                    {
                        ObjectName = arguments.GetString(0),
                    };
                }
            }
        }

        public struct NativeObjectCallRequest : IDisposable
        {
            public const string Name = nameof(NativeObjectCallRequest);

            public int CallId;
            public string ObjectName;
            public string MemberName;
            public ICefListValue ArgumentsIn;
            public object[] ArgumentsOut;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using (var arguments = message.Arguments)
                {
                    arguments.SetInt(0, CallId);
                    arguments.SetString(1, ObjectName);
                    arguments.SetString(2, MemberName);
                    arguments.SetList(3, ArgumentsIn);
                    ArgumentsIn.Dispose();
                    ArgumentsIn = null;
                }
                return message;
            }

            public static NativeObjectCallRequest FromCefMessage(CefProcessMessage message)
            {
                using (var arguments = message.Arguments)
                using (var argsArgs = arguments.GetList(3))
                {
                    return new NativeObjectCallRequest()
                    {
                        CallId = arguments.GetInt(0),
                        ObjectName = arguments.GetString(1),
                        MemberName = arguments.GetString(2),
                        ArgumentsIn = null,
                        ArgumentsOut = CefValueSerialization.DeserializeCefList<object>(argsArgs),
                    };
                }
            }

            public void Dispose()
            {
                ArgumentsIn?.Dispose();
                ArgumentsIn = null;
            }
        }

        public struct NativeObjectCallResult
        {
            public const string Name = nameof(NativeObjectCallResult);

            public int CallId;
            public bool Success;
            public CefValueHolder Result;
            public string Exception;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);

                using (var arguments = message.Arguments)
                {
                    arguments.SetInt(0, CallId);
                    arguments.SetBool(1, Success);
                    Result?.AssignToListAndClearReference(arguments, 2);
                    arguments.SetString(3, Exception);
                }
                return message;
            }

            public static NativeObjectCallResult FromCefMessage(CefProcessMessage message)
            {
                using (var arguments = message.Arguments)
                {
                    return new NativeObjectCallResult()
                    {
                        CallId = arguments.GetInt(0),
                        Success = arguments.GetBool(1),
                        Result = new CefValueHolder(arguments.GetValue(2)),
                        Exception = arguments.GetString(3)
                    };
                }
            }
        }

        public struct JsContextCreated
        {
            public const string Name = nameof(JsContextCreated);

            public CefProcessMessage ToCefProcessMessage()
            {
                return CefProcessMessage.Create(Name);
            }

            public static JsContextCreated FromCefMessage(CefProcessMessage message)
            {
                return new JsContextCreated();
            }
        }

        public struct JsContextReleased
        {
            public const string Name = nameof(JsContextReleased);

            public CefProcessMessage ToCefProcessMessage()
            {
                return CefProcessMessage.Create(Name);
            }

            public static JsContextReleased FromCefMessage(CefProcessMessage message)
            {
                return new JsContextReleased();
            }
        }

        public struct JsUncaughtException
        {
            public const string Name = nameof(JsUncaughtException);

            public string Message;
            public JsStackFrame[] StackFrames;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using (var arguments = message.Arguments)
                {
                    arguments.SetString(0, Message);

                    using (var frames = CefListValue.Create())
                    {
                        for (var i = 0; i < StackFrames.Length; i++)
                        {
                            frames.SetList(i, StackFrames[i].ToCefValue());
                        }

                        arguments.SetList(1, frames);
                    }
                    return message;
                }
            }

            public static JsUncaughtException FromCefMessage(CefProcessMessage message)
            {
                using (var arguments = message.Arguments)
                using (var cefFrames = arguments.GetList(1))
                {
                    var frames = new JsStackFrame[cefFrames.Count];
                    for (var i = 0; i < cefFrames.Count; i++)
                    {
                        using (var cefFrame = cefFrames.GetList(i))
                        {
                            frames[i] = JsStackFrame.FromCefValue(cefFrame);
                        }
                    }
                    return new JsUncaughtException()
                    {
                        Message = arguments.GetString(0),
                        StackFrames = frames
                    };
                }
            }
        }

        public struct JsStackFrame
        {
            public int Column;
            public string FunctionName;
            public int LineNumber;
            public string ScriptNameOrSourceUrl;

            internal CefListValue ToCefValue()
            {
                var result = CefListValue.Create();
                result.SetString(0, FunctionName);
                result.SetString(1, ScriptNameOrSourceUrl);
                result.SetInt(2, LineNumber);
                result.SetInt(3, Column);
                return result;
            }

            internal static JsStackFrame FromCefValue(ICefListValue frame)
            {
                return new JsStackFrame()
                {
                    FunctionName = frame.GetString(0),
                    ScriptNameOrSourceUrl = frame.GetString(1),
                    LineNumber = frame.GetInt(2),
                    Column = frame.GetInt(3)
                };
            }
        }

        public struct UnhandledException
        {
            public const string Name = nameof(UnhandledException);

            public string ExceptionType;
            public string Message;
            public string StackTrace;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using (var arguments = message.Arguments)
                {
                    arguments.SetString(0, ExceptionType);
                    arguments.SetString(1, Message);
                    arguments.SetString(2, StackTrace);
                }
                return message;
            }

            public static UnhandledException FromCefMessage(CefProcessMessage message)
            {
                using (var arguments = message.Arguments)
                {
                    return new UnhandledException()
                    {
                        ExceptionType = arguments.GetString(0),
                        Message = arguments.GetString(1),
                        StackTrace = arguments.GetString(2),
                    };
                }
            }
        }
    }
}
