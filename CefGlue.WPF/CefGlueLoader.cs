using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.WPF
{
    public static class CefGlueLoader
    {
        public static void Initialize(string[] args, CefSettings settings = null)
        {
            CommonCefApp.Run(args, settings);
        }
    }          
}
