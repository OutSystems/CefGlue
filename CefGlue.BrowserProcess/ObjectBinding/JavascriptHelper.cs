namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal static partial class JavascriptHelper
    {
        private const string GlobalObjectName = "cefglue";
        private const string PromiseFactoryFunctionName = "createPromise";
        private const string BindNativeFunctionName = "Bind";
        private const string UnbindNativeFunctionName = "Unbind";

        private static readonly string CefGlueGlobal =
            "var " + GlobalObjectName + ";" +
            "if (!" + GlobalObjectName + ")" +
            "    " + GlobalObjectName + " = {" +
            "        " + PromiseFactoryFunctionName + ": function() {" +
            "            let result = {};" +
            "            let promise = new Promise(function(resolve, reject) {" +
            "                result.resolve = resolve;" +
            "                result.reject = reject;" +
            "            });" +
            "            result.promise = promise;" +
            "            return result;" +
            "        }," +
            "        checkObjectBound: function(objName) {" +
            "            native function " + BindNativeFunctionName + "();" +
            "            if (window.hasOwnProperty(objName))" + // quick check
            "                return Promise.resolve(true);" +
            "            return " + BindNativeFunctionName + "(objName);" +
            "        }," +
            "        deleteObjectBound: function(objName) {" +
            "            native function " + UnbindNativeFunctionName + "();" +
            "            " + UnbindNativeFunctionName + "(objName);" +
            "        }" +
            "    };";

        public static void Register(INativeObjectRegistry nativeObjectRegistry)
        {
            // TODO global name
            CefRuntime.RegisterExtension("cefglue", CefGlueGlobal, new V8BuiltinFunctionHandler(nativeObjectRegistry));
        }

        public static PromiseHolder CreatePromise(this CefV8Context context)
        {
            using (var global = context.GetGlobal())
            using (var cefGlueGlobal = global.GetValue(GlobalObjectName)) // TODO what if cefGlueGlobal == null?
            using (var promiseFactory = cefGlueGlobal.GetValue(PromiseFactoryFunctionName))
            using (var promiseData = promiseFactory.ExecuteFunctionWithContext(context, null, new CefV8Value[0])) // create a promise and return the resolve and reject callbacks
            {
                var promise = promiseData.GetValue("promise");
                var resolve = promiseData.GetValue("resolve");
                var reject = promiseData.GetValue("reject");

                return new PromiseHolder(promise, resolve, reject, context);
            }
        }
    }
}
