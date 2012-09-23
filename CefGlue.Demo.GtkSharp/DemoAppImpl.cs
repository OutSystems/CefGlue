namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gtk;

    internal sealed class DemoAppImpl : DemoApp
    {
        protected override void PlatformInitialize()
        {
            Application.Init();
        }

        protected override void PlatformShutdown()
        {
        }

        protected override void PlatformRunMessageLoop()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.Windows) Application.Run();
            else CefRuntime.RunMessageLoop();
        }

        protected override void PlatformQuitMessageLoop()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.Windows) Application.Quit();
            else CefRuntime.QuitMessageLoop();
        }

        protected override IMainView CreateMainView(MenuItem[] menuItems)
        {
            return new MainViewImpl(this, menuItems);
        }
    }
}
