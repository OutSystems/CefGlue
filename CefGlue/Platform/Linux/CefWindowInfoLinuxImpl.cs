namespace Xilium.CefGlue.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;
    using Xilium.CefGlue.Interop;
    using Xilium.CefGlue.Platform.Windows;

    internal unsafe sealed class CefWindowInfoLinuxImpl : CefWindowInfo
    {
        private cef_window_info_t_linux* _self;

        public CefWindowInfoLinuxImpl()
            : base(true)
        {
            _self = cef_window_info_t_linux.Alloc();
        }

        public CefWindowInfoLinuxImpl(cef_window_info_t* ptr)
            : base(false)
        {
            if (CefRuntime.Platform != CefRuntimePlatform.Linux)
                throw new InvalidOperationException();

            _self = (cef_window_info_t_linux*)ptr;
        }

        internal override cef_window_info_t* GetNativePointer()
        {
            return (cef_window_info_t*)_self;
        }

        protected internal override void DisposeNativePointer()
        {
            cef_window_info_t_linux.Free(_self);
            _self = null;
        }

        public override IntPtr ParentHandle
        {
            get { ThrowIfDisposed(); return _self->parent_widget; }
            set { ThrowIfDisposed(); _self->parent_widget = value; }
        }

        public override IntPtr Handle
        {
            get { ThrowIfDisposed(); return _self->widget; }
            set { ThrowIfDisposed(); _self->widget = value; }
        }

        public override string Name
        {
            get { return default(string); }
            set { }
        }

        public override int X
        {
            get { return default(int); }
            set { }
        }

        public override int Y
        {
            get { return default(int); }
            set { }
        }

        public override int Width
        {
            get { return default(int); }
            set { }
        }

        public override int Height
        {
            get { return default(int); }
            set { }
        }

        public override WindowStyle Style
        {
            get { return default(WindowStyle); }
            set { }
        }

        public override WindowStyleEx StyleEx
        {
            get { return default(WindowStyleEx); }
            set { }
        }

        public override IntPtr MenuHandle
        {
            get { return default(IntPtr); }
            set { }
        }

        public override bool Hidden
        {
            get { return default(bool); }
            set { }
        }

        public override bool WindowRenderingDisabled
        {
            get { ThrowIfDisposed(); return _self->window_rendering_disabled; }
            set { ThrowIfDisposed(); _self->window_rendering_disabled = value; }
        }

        public override bool TransparentPainting
        {
            get { ThrowIfDisposed(); return _self->window_rendering_disabled; }
            set { ThrowIfDisposed(); _self->window_rendering_disabled = value; }
        }
    }
}
