//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Xilium.CefGlue.Interop;
    
    // Role: HANDLER
    public abstract unsafe partial class CefRenderHandler
    {
        private static Dictionary<IntPtr, CefRenderHandler> _roots = new Dictionary<IntPtr, CefRenderHandler>();
        
        private int _refct;
        private cef_render_handler_t* _self;
        
        protected object SyncRoot { get { return this; } }
        
        private cef_render_handler_t.add_ref_delegate _ds0;
        private cef_render_handler_t.release_delegate _ds1;
        private cef_render_handler_t.has_one_ref_delegate _ds2;
        private cef_render_handler_t.has_at_least_one_ref_delegate _ds3;
        private cef_render_handler_t.get_accessibility_handler_delegate _ds4;
        private cef_render_handler_t.get_root_screen_rect_delegate _ds5;
        private cef_render_handler_t.get_view_rect_delegate _ds6;
        private cef_render_handler_t.get_screen_point_delegate _ds7;
        private cef_render_handler_t.get_screen_info_delegate _ds8;
        private cef_render_handler_t.on_popup_show_delegate _ds9;
        private cef_render_handler_t.on_popup_size_delegate _dsa;
        private cef_render_handler_t.on_paint_delegate _dsb;
        private cef_render_handler_t.on_accelerated_paint_delegate _dsc;
        private cef_render_handler_t.get_touch_handle_size_delegate _dsd;
        private cef_render_handler_t.on_touch_handle_state_changed_delegate _dse;
        private cef_render_handler_t.start_dragging_delegate _dsf;
        private cef_render_handler_t.update_drag_cursor_delegate _ds10;
        private cef_render_handler_t.on_scroll_offset_changed_delegate _ds11;
        private cef_render_handler_t.on_ime_composition_range_changed_delegate _ds12;
        private cef_render_handler_t.on_text_selection_changed_delegate _ds13;
        private cef_render_handler_t.on_virtual_keyboard_requested_delegate _ds14;
        
        protected CefRenderHandler()
        {
            _self = cef_render_handler_t.Alloc();
        
            _ds0 = new cef_render_handler_t.add_ref_delegate(add_ref);
            _self->_base._add_ref = Marshal.GetFunctionPointerForDelegate(_ds0);
            _ds1 = new cef_render_handler_t.release_delegate(release);
            _self->_base._release = Marshal.GetFunctionPointerForDelegate(_ds1);
            _ds2 = new cef_render_handler_t.has_one_ref_delegate(has_one_ref);
            _self->_base._has_one_ref = Marshal.GetFunctionPointerForDelegate(_ds2);
            _ds3 = new cef_render_handler_t.has_at_least_one_ref_delegate(has_at_least_one_ref);
            _self->_base._has_at_least_one_ref = Marshal.GetFunctionPointerForDelegate(_ds3);
            _ds4 = new cef_render_handler_t.get_accessibility_handler_delegate(get_accessibility_handler);
            _self->_get_accessibility_handler = Marshal.GetFunctionPointerForDelegate(_ds4);
            _ds5 = new cef_render_handler_t.get_root_screen_rect_delegate(get_root_screen_rect);
            _self->_get_root_screen_rect = Marshal.GetFunctionPointerForDelegate(_ds5);
            _ds6 = new cef_render_handler_t.get_view_rect_delegate(get_view_rect);
            _self->_get_view_rect = Marshal.GetFunctionPointerForDelegate(_ds6);
            _ds7 = new cef_render_handler_t.get_screen_point_delegate(get_screen_point);
            _self->_get_screen_point = Marshal.GetFunctionPointerForDelegate(_ds7);
            _ds8 = new cef_render_handler_t.get_screen_info_delegate(get_screen_info);
            _self->_get_screen_info = Marshal.GetFunctionPointerForDelegate(_ds8);
            _ds9 = new cef_render_handler_t.on_popup_show_delegate(on_popup_show);
            _self->_on_popup_show = Marshal.GetFunctionPointerForDelegate(_ds9);
            _dsa = new cef_render_handler_t.on_popup_size_delegate(on_popup_size);
            _self->_on_popup_size = Marshal.GetFunctionPointerForDelegate(_dsa);
            _dsb = new cef_render_handler_t.on_paint_delegate(on_paint);
            _self->_on_paint = Marshal.GetFunctionPointerForDelegate(_dsb);
            _dsc = new cef_render_handler_t.on_accelerated_paint_delegate(on_accelerated_paint);
            _self->_on_accelerated_paint = Marshal.GetFunctionPointerForDelegate(_dsc);
            _dsd = new cef_render_handler_t.get_touch_handle_size_delegate(get_touch_handle_size);
            _self->_get_touch_handle_size = Marshal.GetFunctionPointerForDelegate(_dsd);
            _dse = new cef_render_handler_t.on_touch_handle_state_changed_delegate(on_touch_handle_state_changed);
            _self->_on_touch_handle_state_changed = Marshal.GetFunctionPointerForDelegate(_dse);
            _dsf = new cef_render_handler_t.start_dragging_delegate(start_dragging);
            _self->_start_dragging = Marshal.GetFunctionPointerForDelegate(_dsf);
            _ds10 = new cef_render_handler_t.update_drag_cursor_delegate(update_drag_cursor);
            _self->_update_drag_cursor = Marshal.GetFunctionPointerForDelegate(_ds10);
            _ds11 = new cef_render_handler_t.on_scroll_offset_changed_delegate(on_scroll_offset_changed);
            _self->_on_scroll_offset_changed = Marshal.GetFunctionPointerForDelegate(_ds11);
            _ds12 = new cef_render_handler_t.on_ime_composition_range_changed_delegate(on_ime_composition_range_changed);
            _self->_on_ime_composition_range_changed = Marshal.GetFunctionPointerForDelegate(_ds12);
            _ds13 = new cef_render_handler_t.on_text_selection_changed_delegate(on_text_selection_changed);
            _self->_on_text_selection_changed = Marshal.GetFunctionPointerForDelegate(_ds13);
            _ds14 = new cef_render_handler_t.on_virtual_keyboard_requested_delegate(on_virtual_keyboard_requested);
            _self->_on_virtual_keyboard_requested = Marshal.GetFunctionPointerForDelegate(_ds14);
        }
        
        ~CefRenderHandler()
        {
            Dispose(false);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_self != null)
            {
                cef_render_handler_t.Free(_self);
                _self = null;
            }
        }
        
        private void add_ref(cef_render_handler_t* self)
        {
            lock (SyncRoot)
            {
                var result = ++_refct;
                if (result == 1)
                {
                    lock (_roots) { _roots.Add((IntPtr)_self, this); }
                }
            }
        }
        
        private int release(cef_render_handler_t* self)
        {
            lock (SyncRoot)
            {
                var result = --_refct;
                if (result == 0)
                {
                    lock (_roots) { _roots.Remove((IntPtr)_self); }
                    return 1;
                }
                return 0;
            }
        }
        
        private int has_one_ref(cef_render_handler_t* self)
        {
            lock (SyncRoot) { return _refct == 1 ? 1 : 0; }
        }
        
        private int has_at_least_one_ref(cef_render_handler_t* self)
        {
            lock (SyncRoot) { return _refct != 0 ? 1 : 0; }
        }
        
        internal cef_render_handler_t* ToNative()
        {
            add_ref(_self);
            return _self;
        }
        
        [Conditional("DEBUG")]
        private void CheckSelf(cef_render_handler_t* self)
        {
            if (_self != self) throw ExceptionBuilder.InvalidSelfReference();
        }
        
    }
}
