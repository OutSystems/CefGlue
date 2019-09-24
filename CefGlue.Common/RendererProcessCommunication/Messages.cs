using Xilium.CefGlue.Common.Serialization;

namespace Xilium.CefGlue.Common.RendererProcessCommunication
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
            public string FrameId;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetInt(0, TaskId);
                arguments.SetString(1, FrameId);
                arguments.SetString(2, Script);
                arguments.SetString(3, Url);
                arguments.SetInt(4, Line);
                return message;
            }

            public static JsEvaluationRequest FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new JsEvaluationRequest()
                {
                    TaskId = arguments.GetInt(0),
                    FrameId = arguments.GetString(1),
                    Script = arguments.GetString(2),
                    Url = arguments.GetString(3),
                    Line = arguments.GetInt(4)
                };
            }
        }

        public struct JsEvaluationResult
        {
            public const string Name = nameof(JsEvaluationResult);

            public int TaskId;
            public bool Success;
            public string Exception;
            public object Result { get; private set; }

            public CefProcessMessage ToCefProcessMessageWithResult(CefV8Value result)
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetInt(0, TaskId);
                arguments.SetBool(1, Success);
                if (result != null)
                {
                    V8ValueSerialization.SerializeV8Object(result, new CefListWrapper(arguments, 2));
                }
                else
                {
                    arguments.SetNull(2);
                }
                arguments.SetString(3, Exception);
                return message;
            }

            public static JsEvaluationResult FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new JsEvaluationResult()
                {
                    TaskId = arguments.GetInt(0),
                    Success = arguments.GetBool(1),
                    Result = CefValueSerialization.DeserializeCefValue(arguments.GetValue(2)),
                    Exception = arguments.GetString(3)
                };
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
                var arguments = message.Arguments;
                arguments.SetString(0, ObjectName);

                var methods = CefListValue.Create();
                for (var i = 0; i < MethodsNames.Length; i++)
                {
                    methods.SetString(i, MethodsNames[i]);
                }

                arguments.SetList(1, methods);
                return message;
            }

            public static NativeObjectRegistrationRequest FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new NativeObjectRegistrationRequest()
                {
                    ObjectName = arguments.GetString(0),
                    MethodsNames = CefValueSerialization.DeserializeCefList<string>(arguments.GetList(1))
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
                var arguments = message.Arguments;
                arguments.SetString(0, ObjectName);
                return message;
            }

            public static NativeObjectRegistrationRequest FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new NativeObjectRegistrationRequest()
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
            public CefListValue Arguments;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetInt(0, CallId);
                arguments.SetString(1, ObjectName);
                arguments.SetString(2, MemberName);
                arguments.SetList(3, Arguments);
                return message;
            }

            public static NativeObjectCallRequest FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new NativeObjectCallRequest()
                {
                    CallId = arguments.GetInt(0),
                    ObjectName = arguments.GetString(1),
                    MemberName = arguments.GetString(2),
                    Arguments = arguments.GetList(3)
                };
            }
        }

        public struct NativeObjectCallResult
        {
            public const string Name = nameof(NativeObjectCallResult);

            public int CallId;
            public bool Success;
            public object Result;
            public string Exception;

            public CefValue CefResult => (CefValue)Result;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);

                var arguments = message.Arguments;
                arguments.SetInt(0, CallId);
                arguments.SetBool(1, Success);
                CefValueSerialization.Serialize(Result, new CefListWrapper(arguments, 2));
                arguments.SetString(3, Exception);
                return message;
            }

            public static NativeObjectCallResult FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new NativeObjectCallResult()
                {
                    CallId = arguments.GetInt(0),
                    Success = arguments.GetBool(1),
                    Result = arguments.GetValue(2),
                    Exception = arguments.GetString(3)
                };
            }
        }

        public struct JsContextCreated
        {
            public const string Name = nameof(JsContextCreated);

            public string FrameId;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetString(0, FrameId);
                return message;
            }

            public static JsContextCreated FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new JsContextCreated()
                {
                    FrameId = arguments.GetString(0),
                };
            }
        }

        public struct JsContextReleased
        {
            public const string Name = nameof(JsContextReleased);

            public string FrameId;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetString(0, FrameId);
                return message;
            }

            public static JsContextReleased FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new JsContextReleased()
                {
                    FrameId = arguments.GetString(0),
                };
            }
        }

        public struct JsUncaughtException
        {
            public const string Name = nameof(JsUncaughtException);

            public string FrameId;
            public string Message;
            public JsStackFrame[] StackFrames;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetString(0, FrameId);
                arguments.SetString(1, Message);

                var frames = CefListValue.Create();
                for (var i = 0; i < StackFrames.Length; i++)
                {
                    frames.SetList(i, StackFrames[i].ToCefValue());
                }

                arguments.SetList(2, frames);
                return message;
            }

            public static JsUncaughtException FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                var cefFrames = arguments.GetList(2);
                var frames = new JsStackFrame[cefFrames.Count];
                for (var i = 0; i < cefFrames.Count; i++)
                {
                    frames[i] = JsStackFrame.FromCefValue(cefFrames.GetList(i));
                }
                return new JsUncaughtException()
                {
                    FrameId = arguments.GetString(0),
                    Message = arguments.GetString(1),
                    StackFrames = frames
                };
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

            internal static JsStackFrame FromCefValue(CefListValue frame)
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
    }
}
