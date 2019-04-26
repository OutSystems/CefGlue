using Avalonia;
using Xilium.CefGlue.Avalonia;

namespace Xilium.CefGlue.Demo.Avalonia
{
    class Program
    {
        static int Main(string[] args)
        {
            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .UseSkia()
                      .ConfigureCefGlue(args)
                      .Start<MainWindow>();
            return 0;
        }
    }
}
