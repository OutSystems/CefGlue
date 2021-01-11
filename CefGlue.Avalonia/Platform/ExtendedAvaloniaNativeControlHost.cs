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

        private bool _isAttached;

        public ExtendedAvaloniaNativeControlHost(IntPtr browserHandle)
        {
            _browserHandle = browserHandle;

            if (CefRuntime.Platform == CefRuntimePlatform.MacOSX)
            {
                // In OSX we need to force update of the browser bounds: https://magpcss.org/ceforum/viewtopic.php?f=6&t=16341
                void UpdateNativeControlBounds(AvaloniaPropertyChangedEventArgs ea)
                {
                    if (!_isAttached)
                    {
                        return;
                    }
                    var transformedBoundsEventArg = (AvaloniaPropertyChangedEventArgs<TransformedBounds?>)ea;
                    if (transformedBoundsEventArg.NewValue.Value?.Bounds.IsEmpty == false)
                    {
                        // only update when not empty otherwise consequent updates might not have effect
                        TryUpdateNativeControlPosition();
                    }
                }

                this.GetPropertyChangedObservable(TransformedBoundsProperty).Subscribe(UpdateNativeControlBounds);
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _isAttached = true;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _isAttached = false;
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
