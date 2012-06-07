namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue.Interop;

    public sealed unsafe class CefPopupFeatures
    {
        private cef_popup_features_t* _self;

        internal CefPopupFeatures(cef_popup_features_t* ptr)
        {
            _self = ptr;
        }

        internal void Dispose()
        {
            _self = null;
        }

        public int? X
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->xSet ? (int?)_self->x : null;
            }
        }

        public int? Y
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->ySet ? (int?)_self->y : null;
            }
        }

        public int? Width
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->widthSet ? (int?)_self->width : null;
            }
        }

        public int? Height
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->heightSet ? (int?)_self->height : null;
            }
        }

        public bool MenuBarVisible
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->menuBarVisible;
            }
        }

        public bool StatusBarVisible
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->statusBarVisible;
            }
        }

        public bool ToolBarVisible
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->toolBarVisible;
            }
        }

        public bool LocationBarVisible
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->locationBarVisible;
            }
        }

        public bool ScrollbarsVisible
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->scrollbarsVisible;
            }
        }

        public bool Resizable
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->resizable;
            }
        }

        public bool Fullscreen
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->fullscreen;
            }
        }

        public bool Dialog
        {
            get
            {
                ThrowIfObjectDisposed();
                return _self->dialog;
            }
        }

        public string[] AdditionalFeatures
        {
            get
            {
                return cef_string_list.ToArray(_self->additionalFeatures);
            }
        }

        private void ThrowIfObjectDisposed()
        {
            if (_self == null) throw ExceptionBuilder.ObjectDisposed();
        }
    }
}
