using Avalonia.Controls;
using Avalonia.VisualTree;
using Xilium.CefGlue.Avalonia.Platform;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Avalonia
{
    /// <summary>
    /// The Avalonia CEF browser.
    /// </summary>
    public class AvaloniaCefBrowser : BaseCefBrowser
    {
        static AvaloniaCefBrowser()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.MacOSX && !CefRuntimeLoader.IsLoaded)
            {
                CefRuntimeLoader.Load(new AvaloniaBrowserProcessHandler());
            }
        }

        internal override Common.Platform.IControl CreateControl()
        {
            return new AvaloniaControl(this, GetHostingWindow);
        }


        internal override IOffScreenControlHost CreateOffScreenControlHost()
        {
            return new AvaloniaOffScreenControlHost(this, GetHostingWindow);
        }

        internal override IOffScreenPopupHost CreatePopupHost()
        {
            var popup = new ExtendedAvaloniaPopup
            {
                PlacementTarget = this
            };
            return new AvaloniaPopup(popup, GetHostingWindow);
        }

        protected virtual Window GetHostingWindow()
        {
            return this.GetVisualRoot() as Window;
        }
    }
}
