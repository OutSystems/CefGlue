using System;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal interface IHandleHolder : IDisposable
    {
        IntPtr Handle { get; }
    }
}
