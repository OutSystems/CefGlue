using System.Collections.Generic;

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
            public byte[] Result;
            public string Exception;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using (var arguments = message.Arguments)
                {
                    arguments.SetInt(0, TaskId);
                    arguments.SetBool(1, Success);
                    arguments.SetByteArray(2, Result);
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
                        Result = arguments.GetByteArray(2),
                        Exception = arguments.GetString(3)
                    };
                }
            }
        }

        public struct NativeObjectRegistrationRequest
        {
            public const string Name = nameof(NativeObjectRegistrationRequest);

            public ObjectInfo ObjectInfo;

            public CefProcessMessage ToCefProcessMessage()
            {
                using var methodList = CefListValue.Create();
                var methods = ObjectInfo.Methods;
                for (int index = 0; index < methods.Length; index++)
                {
                    using var value = CefListValue.Create();
                    var method = methods[index];
                    value.SetString(0, method.Name);
                    value.SetInt(1, method.ParameterCount);
                    methodList.SetList(index, value);
                }

                var message = CefProcessMessage.Create(Name);
                using var arguments = message.Arguments;
                arguments.SetString(0, ObjectInfo.Name);
                arguments.SetList(1, methodList);
                return message;
            }

            public static NativeObjectRegistrationRequest FromCefMessage(CefProcessMessage message)
            {
                using var arguments = message.Arguments;
                var methods = new List<MethodInfo>();

                var methodList = arguments.GetList(1);
                for (int index = 0; index < methodList.Count; index++)
                {
                    using var value = methodList.GetList(index);
                    methods.Add(new MethodInfo(value.GetString(0), value.GetInt(1)));
                }

                return new NativeObjectRegistrationRequest
                {
                    ObjectInfo = new ObjectInfo(arguments.GetString(0), methods.ToArray())
                };
            }
        }

        public struct NativeObjectUnregistrationRequest
        {
            public const string Name = nameof(NativeObjectUnregistrationRequest);

            public string ObjectName;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                using var arguments = message.Arguments;

                arguments.SetString(0, ObjectName);

                return message;
            }

            public static NativeObjectUnregistrationRequest FromCefMessage(CefProcessMessage message)
            {
                using var arguments = message.Arguments;
                return new NativeObjectUnregistrationRequest()
                {
                    ObjectName = arguments.GetString(0),
                };
            }
        }

        public struct NativeObjectCallRequest
        {
            public const string Name = nameof(NativeObjectCallRequest);

            public int CallId;
            public string ObjectName;
            public string MemberName;
            public byte[] Arguments;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetInt(0, CallId);
                arguments.SetString(1, ObjectName);
                arguments.SetString(2, MemberName);
                arguments.SetByteArray(3, Arguments);
                return message;
            }

            public static NativeObjectCallRequest FromCefMessage(CefProcessMessage message)
            {
                using var arguments = message.Arguments;
                return new NativeObjectCallRequest()
                {
                    CallId = arguments.GetInt(0),
                    ObjectName = arguments.GetString(1),
                    MemberName = arguments.GetString(2),
                    Arguments = arguments.GetByteArray(3),
                };
            }
        }

        public struct NativeObjectCallResult
        {
            public const string Name = nameof(NativeObjectCallResult);

            public int CallId;
            public bool Success;
            public byte[] Result;
            public string Exception;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);

                using (var arguments = message.Arguments)
                {
                    arguments.SetInt(0, CallId);
                    arguments.SetBool(1, Success);
                    arguments.SetByteArray(2, Result);
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
                        Result = arguments.GetByteArray(2),
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

    public static class CefListExtensions
    {
        public static bool SetByteArray(this ICefListValue valueList, int index, byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                return valueList.SetBinary(index, CefBinaryValue.Create(bytes));
            }

            return valueList.SetNull(index);
        }

        public static byte[] GetByteArray(this ICefListValue valueList, int index)
        {
            CefValueType valueType = valueList.GetValueType(index);
            if (valueType == CefValueType.Null)
            {
                return [];
            }
            else
            {
                return valueList.GetBinary(index).ToArray();
            }
        }
    }
}
