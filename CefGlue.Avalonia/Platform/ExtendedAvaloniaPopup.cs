using System;
using System.Runtime.InteropServices;
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
        public enum ShowWindowCommand
        {
            Hide = 0,
            ShowNoActivate = 4
        }

        // TODO handle other OS platforms
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

        public ExtendedAvaloniaPopup()
        {
            CanResize = false;
            HasSystemDecorations = false;
            Focusable = false;
            Topmost = true;
        }

        /// <summary>
        /// Controls wheteher popup will be activated on show.
        /// </summary>
        public bool ShowActivated { get; set; } = false;
        
        /// <summary>
        /// The element to be used as the positioning reference.
        /// </summary>
        public Control PlacementTarget { get; set; }

        public override void Show()
        {
            // shamelessly copied from avalonia source code
            if (PlatformImpl == null)
            {
                throw new InvalidOperationException("Cannot re-show a closed window.");
            }

            if (IsVisible)
            {
                return;
            }

            // TODO not available!?
            //this.RaiseEvent(new RoutedEventArgs(Window.WindowOpenedEvent));

            EnsureInitialized();
            IsVisible = true;
            LayoutManager.ExecuteInitialLayoutPass(this);

            //ShowInTaskbar = false; // setting this property before will force the window to show

            using (BeginAutoSizing())
            {
                if (!ShowActivated)
                {
                    var handle = PlatformImpl?.Handle.Handle;
                    if (handle != null)
                    {
                        ShowWindow(handle.Value, ShowWindowCommand.ShowNoActivate);
                    }
                }
                else
                {
                    PlatformImpl?.Show();
                }
                Renderer?.Start();
            }
            OnOpened(EventArgs.Empty);
        }
    }
}
