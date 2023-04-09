using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal class ExtendedAvaloniaNativeControlHost : NativeControlHost
    {
        private readonly IntPtr _browserHandle;
        private bool _isAttached;

        public ExtendedAvaloniaNativeControlHost(IntPtr browserHandle)
        {
            _browserHandle = browserHandle;
            
            if (CefRuntime.Platform == CefRuntimePlatform.MacOS)
            {
                // HACK: In OSX we need to force update of the browser bounds: https://magpcss.org/ceforum/viewtopic.php?f=6&t=16341
                void UpdateNativeControlBounds(AvaloniaPropertyChangedEventArgs ea)
                {
                    FixNativeNativeControlBounds();
                }
            
                this.GetPropertyChangedObservable(BoundsProperty).Subscribe(UpdateNativeControlBounds);
            
                AttachedToVisualTree += OnAttachedToVisualTree;
                DetachedFromVisualTree += OnDetachedFromVisualTree;
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
        
        private void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _isAttached = true;
            if (e.Root is WindowBase rootWindow)
            {
                rootWindow.Opened += OnRootWindowOpened;
            }
            FixNativeNativeControlBounds();
        }

        private void OnDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _isAttached = false;
            if (e.Root is WindowBase rootWindow)
            {
                rootWindow.Opened -= OnRootWindowOpened;
            }
        }
        
        private void OnRootWindowOpened(object sender, EventArgs e)
        {
            FixNativeNativeControlBounds();
        }
        
        private void FixNativeNativeControlBounds()
        {
            if ((Bounds.Height != 0 || Bounds.Width != 0) && _isAttached)
            {
                // try delay native host position update, because running right away seems to have no effect sometimes
                DispatcherTimer.RunOnce(() =>
                {
                    if (_isAttached)
                    {
                        TryUpdateNativeControlPosition();
                    }
                }, TimeSpan.FromMilliseconds(500));
            }
        }
    }
}
