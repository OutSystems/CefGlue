using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;

namespace Xilium.CefGlue.Avalonia.Platform
{
    /// <summary>
    /// Enhanced version of an Avalonia popup.
    /// Fixes some problems of the Avalonia Popup implementation.
    /// TODO: 
    /// - window shows a black border: https://github.com/AvaloniaUI/Avalonia/issues/2401
    /// </summary>
    internal class ExtendedAvaloniaPopup : Window
    {
        public ExtendedAvaloniaPopup()
        {
            CanResize = false;
            SystemDecorations = SystemDecorations.None;
            Focusable = false;
            Topmost = true;
            ShowActivated = false;
        }
        
        /// <summary>
        /// The element to be used as the positioning reference.
        /// </summary>
        public Control PlacementTarget { get; set; }

        public new IAvaloniaList<Visual> VisualChildren => base.VisualChildren;
    }
}
