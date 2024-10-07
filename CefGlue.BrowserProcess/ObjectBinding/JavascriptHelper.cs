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
                CefRuntime.RegisterExtension(GlobalObjectName, cefGlueGlobalScript, new V8BuiltinFunctionHandler(nativeObjectRegistry));
            }
        }

        public static PromiseHolder CreatePromise(this CefV8Context context)
        {
            var promiseFactory = GetGlobalInnerValue(context, ScriptFunctions.CreatePromise);
            var promiseData = promiseFactory.ExecuteFunctionWithContext(context, null, []); // create a promise and return the resolve and reject callbacks

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
            var interceptorFactory = GetGlobalInnerValue(context, ScriptFunctions.CreateInterceptor);

            return interceptorFactory.ExecuteFunctionWithContext(context, null, [targetObj]);
        }

        private static CefV8Value GetGlobalInnerValue(CefV8Context context, string innerValueKey)
        {
            var global = context.GetGlobal();
            var cefGlueGlobal = global.GetValue(GlobalObjectName); // TODO what if cefGlueGlobal == null?

            return cefGlueGlobal.GetValue(innerValueKey);
        }

        public static string WrapScriptForEvaluation(string script)
        {
            return $"{GlobalObjectName}.{ScriptFunctions.EvaluateScript}(function() {{{script}\n}})";
        }
    }
}
