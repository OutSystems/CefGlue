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

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetInt(0, TaskId);
                arguments.SetString(1, Script);
                arguments.SetString(2, Url);
                arguments.SetInt(3, Line);
                return message;
            }

            public static JsEvaluationRequest FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new JsEvaluationRequest()
                {
                    TaskId = arguments.GetInt(0),
                    Script = arguments.GetString(1),
                    Url = arguments.GetString(2),
                    Line = arguments.GetInt(3)
                };
            }
        }

        public struct JsEvaluationResult
        {
            public const string Name = nameof(JsEvaluationResult);

            public int TaskId;
            public bool Success;
            public CefValue Result;
            public string Exception;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetInt(0, TaskId);
                arguments.SetBool(1, Success);
                if (Result != null)
                {
                    arguments.SetValue(2, Result);
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
                    Result = arguments.GetValue(2),
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
                for(var i = 0; i < MethodsNames.Length; i++)
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
                    MethodsNames = V8Serialization.DeserializeV8List<string>(arguments.GetList(1))
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
            public CefValue Result;
            public string Exception;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetInt(0, CallId);
                arguments.SetBool(1, Success);
                if (Result != null)
                {
                    arguments.SetValue(2, Result);
                }
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
    }
}
