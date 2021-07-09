using System;
using System.Runtime.InteropServices;

namespace Xilium.CefGlue.Avalonia.Platform.Windows
{
    internal class HostWindow : IHandleHolder
    {
        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
        private static extern bool DestroyWindow(IntPtr hwnd);

        public HostWindow(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; }

        public void Dispose()
        {
            DestroyWindow(Handle);
        }
    }
}
