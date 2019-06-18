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

        public struct JsEvaluationResponse
        {
            public const string Name = nameof(JsEvaluationResponse);

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

            public static JsEvaluationResponse FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new JsEvaluationResponse()
                {
                    TaskId = arguments.GetInt(0),
                    Success = arguments.GetBool(1),
                    Result = arguments.GetValue(2),
                    Exception = arguments.GetString(3)
                };
            }
        }

        public struct JsObjectRegistrationRequest
        {
            public const string Name = nameof(JsObjectRegistrationRequest);

            public int ObjectTrackId;

            public CefProcessMessage ToCefProcessMessage()
            {
                var message = CefProcessMessage.Create(Name);
                var arguments = message.Arguments;
                arguments.SetInt(0, ObjectTrackId);
                return message;
            }

            public static JsObjectRegistrationRequest FromCefMessage(CefProcessMessage message)
            {
                var arguments = message.Arguments;
                return new JsObjectRegistrationRequest()
                {
                    ObjectTrackId = arguments.GetInt(0),
                };
            }
        }
    }
}
