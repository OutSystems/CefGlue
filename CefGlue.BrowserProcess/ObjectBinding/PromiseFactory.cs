namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal static class PromiseFactory
    {
        private const string PromiseFactoryFunctionName = "cefglue_createPromise";
        private static readonly string PromiseFactoryFunction =
        $"function {PromiseFactoryFunctionName}() {{" +
        "   let result = {};" +
        "   let promise = new Promise(function(resolve, reject) {" +
        "       result.resolve = resolve;" +
        "       result.reject = reject;" +
        "   });" +
        "   result.promise = promise;" +
        "   return result;" +
        "}";

        public static void Register()
        {
            CefRuntime.RegisterExtension("cefglue", PromiseFactoryFunction, null);
        }

        public static PromiseHolder CreatePromise()
        {
            var context = CefV8Context.GetCurrentContext();
            var promiseFactory = context.GetGlobal().GetValue(PromiseFactoryFunctionName);

            // create a promise and return the resolve and reject callbacks
            var promiseData = promiseFactory.ExecuteFunctionWithContext(context, null, new CefV8Value[0]);

            var promise = promiseData.GetValue("promise");
            var resolve = promiseData.GetValue("resolve");
            var reject = promiseData.GetValue("reject");

            return new PromiseHolder()
            {
                Promise = promise,
                Resolve = resolve,
                Reject = reject,
                Context = context
            };
        }
    }
}
