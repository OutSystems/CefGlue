namespace Xilium.CefGlue.Platform.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal static partial class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
