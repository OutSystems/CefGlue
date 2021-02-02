using System;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal interface IHost : IDisposable
    {
        IntPtr Handle { get; }
    }
}
