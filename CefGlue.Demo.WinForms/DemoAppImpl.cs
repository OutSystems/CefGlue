namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    internal sealed class DemoAppImpl : DemoApp
    {
        protected override void PlatformInitialize()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        protected override void PlatformShutdown()
        {
        }

        protected override void PlatformRunMessageLoop()
        {
            if (!MultiThreadedMessageLoop)
            {
                Application.Idle += (s, e) => CefRuntime.DoMessageLoopWork();
            }

            Application.Run();
        }

        protected override void PlatformQuitMessageLoop()
        {
            Application.Exit();
        }

        protected override IMainView CreateMainView(MenuItem[] menuItems)
        {
            return new MainViewImpl(this, menuItems);
        }

        protected override void PlatformMessageBox(string message)
        {
            MessageBox.Show(message, "CefGlue Demo Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
