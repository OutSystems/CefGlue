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
                // in osx we need to force an extra update, otherwise the browser will have wrong dimensions when initialized
                IDisposable observable = null;
                void UpdateNativeControlBounds(AvaloniaPropertyChangedEventArgs _)
                {
                    observable.Dispose();
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
