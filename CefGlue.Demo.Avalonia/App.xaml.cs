using Avalonia;
using Avalonia.Markup.Xaml;

namespace Xilium.CefGlue.Demo.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
