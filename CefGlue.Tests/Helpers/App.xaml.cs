using Avalonia;
using Avalonia.Markup.Xaml;

namespace CefGlue.Tests.Helpers
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
