using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal class ExtendedAvaloniaNativeControlHost : NativeControlHost
    {
        private readonly IntPtr _browserHandle;

        public ExtendedAvaloniaNativeControlHost(IntPtr browserHandle)
        {
            _browserHandle = browserHandle;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            if(CefRuntime.Platform == CefRuntimePlatform.MacOSX)
            {
                // In OSX we need to force update of the browser bounds: https://magpcss.org/ceforum/viewtopic.php?f=6&t=16341
                IDisposable observable = null;
                void UpdateNativeControlBounds(AvaloniaPropertyChangedEventArgs _)
                {
                    TryUpdateNativeControlPosition();
                }

                observable = this.GetPropertyChangedObservable(Control.TransformedBoundsProperty).Subscribe(UpdateNativeControlBounds);
            }
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle handle)
        {
            return new PlatformHandle(_browserHandle, "HWND");
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            // do nothing
        }
    }
}
