using System.IO;
using System.Text;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal static partial class JavascriptHelper
    {
        private const string CefGlueGlobalScriptFileName = "CefGlueGlobalScript.js";
        private const string GlobalObjectName = "cefglue";
        private const string PromiseFactoryFunctionName = "createPromise";
        private const string InterceptorFactoryFunctionName = "createInterceptor";
        private const string BindNativeFunctionName = "Bind";
        private const string UnbindNativeFunctionName = "Unbind";
        private const string EvaluateScriptFunctionName = "evaluateScript";

        public static void Register(INativeObjectRegistry nativeObjectRegistry)
        {
            var currentNamespace = typeof(JavascriptHelper).Namespace;
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var stream = new StreamReader(assembly.GetManifestResourceStream($"{currentNamespace}.{CefGlueGlobalScriptFileName}")))
            {
                var cefGlueGlobalScript = ReplaceScriptGlobalVariables(stream.ReadToEnd());
                CefRuntime.RegisterExtension("cefglue", cefGlueGlobalScript, new V8BuiltinFunctionHandler(nativeObjectRegistry));
            }
                
        }

        public static void RegisterJavascriptExecution(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.JsEvaluationRequest.Name, HandleJavascriptEvaluation);
        }

        public static PromiseHolder CreatePromise(this CefV8Context context)
        {
            var global = context.GetGlobal();
            var cefGlueGlobal = global.GetValue(GlobalObjectName); // TODO what if cefGlueGlobal == null?
            var promiseFactory = cefGlueGlobal.GetValue(PromiseFactoryFunctionName);
            var promiseData = promiseFactory.ExecuteFunctionWithContext(context, null, new CefV8Value[0]); // create a promise and return the resolve and reject callbacks

            var promise = promiseData.GetValue("promise");
            var resolve = promiseData.GetValue("resolve");
            var reject = promiseData.GetValue("reject");

            CefObjectTracker.Untrack(promise);
            CefObjectTracker.Untrack(resolve);
            CefObjectTracker.Untrack(reject);

            return new PromiseHolder(promise, resolve, reject, context);
        }

        public static CefV8Value CreateInterceptorObject(this CefV8Context context, CefV8Value targetObj)
        {
            var global = context.GetGlobal();
            var cefGlueGlobal = global.GetValue(GlobalObjectName);
            var interceptorFactory = cefGlueGlobal.GetValue(InterceptorFactoryFunctionName);

            return interceptorFactory.ExecuteFunctionWithContext(context, null, new[] { targetObj });
        }

        private static void HandleJavascriptEvaluation(MessageReceivedEventArgs args)
        {
            var frame = args.Frame;

            using (var context = frame.V8Context.EnterOrFail())
            {
                var message = Messages.JsEvaluationRequest.FromCefMessage(args.Message);

                // TODO - bcs - move method to JavaScriptHelper
                // send script to browser
                var success = context.V8Context.TryEval($"{GlobalObjectName}.{EvaluateScriptFunctionName}(() => ({message.Script}))", message.Url, message.Line, out var value, out var exception);

                var response = new Messages.JsEvaluationResult()
                {
                    TaskId = message.TaskId,
                    Success = success,
                    Exception = success ? null : exception.Message,
                    ResultAsJson = value?.GetStringValue()
                };

                var cefResponseMessage = response.ToCefProcessMessage();
                frame.SendProcessMessage(CefProcessId.Browser, cefResponseMessage);
            }
        }

        private static string ReplaceScriptGlobalVariables(string script)
        {
            var sb = new StringBuilder(script);
            return (sb
                .Replace("$GlobalObjectName$", GlobalObjectName)
                .Replace("$JsonIdAttribute$", JsonAttributes.Id)
                .Replace("$JsonRefAttribute$", JsonAttributes.Ref)
                .Replace("$JsonValuesAttribute$", JsonAttributes.Values)
                .Replace("$DataMarkerLength$", DataMarkers.MarkerLength.ToString())
                .Replace("$StringMarker$", DataMarkers.StringMarker)
                .Replace("$DateTimeMarker$", DataMarkers.DateTimeMarker)
                .Replace("$BinaryMarker$", DataMarkers.BinaryMarker)
                .Replace("$PromiseFactoryFunctionName$", PromiseFactoryFunctionName)
                .Replace("$InterceptorFactoryFunctionName$", InterceptorFactoryFunctionName)
                .Replace("$BindNativeFunctionName$", BindNativeFunctionName)
                .Replace("$UnbindNativeFunctionName$", UnbindNativeFunctionName)
                .Replace("$EvaluateScriptFunctionName$", EvaluateScriptFunctionName)
                ).ToString();
        }
    }
}
