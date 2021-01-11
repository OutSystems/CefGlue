using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.VisualTree;

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
                void UpdateNativeControlBounds(AvaloniaPropertyChangedEventArgs ea)
                {
                    var transformedBoundsEventArg = (AvaloniaPropertyChangedEventArgs<TransformedBounds?>)ea;
                    if (transformedBoundsEventArg.NewValue.Value?.Bounds.IsEmpty == false)
                    {
                        // only update when not empty otherwise consequent updates might not have effect
                        TryUpdateNativeControlPosition();
                    }
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
