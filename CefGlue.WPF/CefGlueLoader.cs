using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.WPF
{
    public static class CefGlueLoader
    {
        public static void Initialize(string[] args)
        {
            var cefApp = new CommonCefApp(args);
            cefApp.Run();
        }
    }          
}
