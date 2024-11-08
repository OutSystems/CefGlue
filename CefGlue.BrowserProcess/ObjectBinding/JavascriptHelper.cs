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
        private const string GlueObjectName = "cefglue";
        private const string BindNativeFunctionName = "Bind";
        private const string UnbindNativeFunctionName = "Unbind";

        public static void Register(INativeObjectRegistry nativeObjectRegistry)
        {
            var currentType = typeof(JavascriptHelper);
            var currentNamespace = currentType.Namespace;
            var currentAssembly = currentType.Assembly;
            using (var stream = new StreamReader(currentAssembly.GetManifestResourceStream($"{currentNamespace}.{CefGlueGlobalScriptFileName}")))
            {
                var cefGlueGlobalScript = stream.ReadToEnd();
                CefRuntime.RegisterExtension(GlueObjectName, cefGlueGlobalScript, new V8BuiltinFunctionHandler(nativeObjectRegistry));
            }
        }

        public static PromiseHolder CreatePromise(this CefV8Context context, CefV8Value serializer = null)
        {
            var promiseFactory = GetGlueValue(context, JsGlueFunction.CreatePromise);
            var promiseData = promiseFactory.ExecuteFunctionWithContext(context, null, [serializer ?? CefV8Value.CreateUndefined()]); // create a promise and return the resolve and reject callbacks

            var promise = promiseData.GetValue("promise");
            var resolve = promiseData.GetValue("resolve");
            var reject = promiseData.GetValue("reject");

            CefObjectTracker.Untrack(promise);
            CefObjectTracker.Untrack(resolve);
            CefObjectTracker.Untrack(reject);

            return new PromiseHolder(promise, resolve, reject, context);
        }

        public static CefV8Value CreateInterceptorObject(this CefV8Context context, CefV8Value targetObj, CefV8Value serializer)
        {
            var interceptorFactory = context.GetGlueValue(JsGlueFunction.CreateInterceptor);

            return interceptorFactory.ExecuteFunctionWithContext(context, null, [targetObj, serializer]);
        }

        public static CefV8Value GetGlueValue(this CefV8Value global, string innerValueKey)
        {
            return global.GetValue(GlueObjectName).GetValue(innerValueKey); // TODO what if cefGlueGlobal == null?
        }

        public static CefV8Value GetGlueValue(this CefV8Context context, string innerValueKey)
        {
            return context.GetGlobal().GetGlueValue(innerValueKey);
        }

        public static string WrapScriptForEvaluation(string script)
        {
            return $"{GlueObjectName}.{JsGlueFunction.EvaluateScript}(function() {{{script}\n}})";
        }

        public static void SetDefaultSerializer(this CefV8Context context, CefV8Value serializer)
        {
            var serializerFunction = context.GetGlueValue(JsGlueFunction.SetDefaultSerializer);
            serializerFunction.ExecuteFunctionWithContext(context, null, [serializer]);
        }

        public static CefV8Value GetDefaultSerializer(this CefV8Context context)
        {
            var serializerFunction = context.GetGlueValue(JsGlueFunction.GetDefaultSerializer);
            return serializerFunction.ExecuteFunctionWithContext(context, null, []);
        }
    }
}
