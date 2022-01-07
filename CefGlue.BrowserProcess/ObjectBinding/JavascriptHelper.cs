using Xilium.CefGlue.Common.Shared.Serialization;

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
            "    " + GlobalObjectName + " = (function() {" +
            "        const stringMarker = \"" + DataMarkers.StringMarker + "\";" +
            "        const dateTimeMarker = \"" + DataMarkers.DateTimeMarker + "\";" +
            "        const binaryMarker = \"" + DataMarkers.BinaryMarker +" \";" +
            "        const stringTypeName = \"string\";" + 
            "        function isString(value) {" +
            "            return typeof value === stringTypeName;" +
            "        }" +
            "        function convertBase64ToBinary(value) {" +
            "            const byteCharacters = atob(value);" +
            "            const byteArray = new Array(byteCharacters.length);" +
            "            for (let i = 0; i < byteCharacters.length; i++)" +
            "                byteArray[i] = byteCharacters.charCodeAt(i);" +
            "            return byteArray;" +
            "        }" +
            "        function revive(name, value) {" +
            "            if (isString(value)) {" +
            "                if (value.startsWith(stringMarker))" +
            "                    return value.substring(" + DataMarkers.StringMarker.Length + ");" +
            "                if (value.startsWith(dateTimeMarker))" +
            "                    return new Date(value.substring(" + DataMarkers.DateTimeMarker.Length + "));" +
            "                if (value.startsWith(binaryMarker))" +
            "                    return convertBase64ToBinary(value.substring(" + DataMarkers.BinaryMarker.Length + "));" +
            "            }" +
            "            return value;" +
            "        }" +
            "        function parseResult(result) {" +
            "            return isString(result) ? JSON.parse(result, revive) : result;"+
            "        }" +
            "        return {" +
            "            " + PromiseFactoryFunctionName + ": function() {" +
            "                const result = {};" +
            "                const promise = new Promise(function(resolve, reject) {" +
            "                    result.resolve = function(result) {" +
            "                        resolve(parseResult(result));" +
            "                    };" +
            "                    result.reject = reject;" +
            "                });" +
            "                result.promise = promise;" +
            "                return result;" +
            "            }," +
            "            checkObjectBound: function(objName) {" +
            "                native function " + BindNativeFunctionName + "();" +
            "                if (window.hasOwnProperty(objName))" + // quick check
            "                    return Promise.resolve(true);" +
            "                return " + BindNativeFunctionName + "(objName);" +
            "            }," +
            "            deleteObjectBound: function(objName) {" +
            "                native function " + UnbindNativeFunctionName + "();" +
            "                " + UnbindNativeFunctionName + "(objName);" +
            "            }" +
            "        };" +
            "    })();";

        public static void Register(INativeObjectRegistry nativeObjectRegistry)
        {
            // TODO global name
            CefRuntime.RegisterExtension("cefglue", CefGlueGlobal, new V8BuiltinFunctionHandler(nativeObjectRegistry));
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
    }
}
