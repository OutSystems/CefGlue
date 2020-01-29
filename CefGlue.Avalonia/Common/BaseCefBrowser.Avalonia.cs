using Avalonia.Controls;
using Avalonia.Styling;
using System;

namespace Xilium.CefGlue.Common
{
    partial class BaseCefBrowser : ContentControl, IStyleable
    {
        Type IStyleable.StyleKey => typeof(ContentControl);
    }
}
