using Avalonia.Controls;
using Avalonia.Styling;
using System;

namespace Xilium.CefGlue.Common
{
    partial class BaseCefBrowser : NativeControlHost, IStyleable
    {
        Type IStyleable.StyleKey => typeof(NativeControlHost);
    }
}
