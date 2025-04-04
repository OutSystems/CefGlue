namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to handle events when window rendering is disabled.
    /// The methods of this class will be called on the UI thread.
    /// </summary>
    public abstract unsafe partial class CefRenderHandler
    {
        private cef_accessibility_handler_t* get_accessibility_handler(cef_render_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.GetAccessibilityHandler
        }
        
        /// <summary>
        /// Return the handler for accessibility notifications. If no handler is
        /// provided the default implementation will be used.
        /// </summary>
        // protected abstract cef_accessibility_handler_t* GetAccessibilityHandler();
        
        private int get_root_screen_rect(cef_render_handler_t* self, cef_browser_t* browser, cef_rect_t* rect)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.GetRootScreenRect
        }
        
        /// <summary>
        /// Called to retrieve the root window rectangle in screen DIP coordinates.
        /// Return true if the rectangle was provided. If this method returns false
        /// the rectangle from GetViewRect will be used.
        /// </summary>
        // protected abstract int GetRootScreenRect(cef_browser_t* browser, cef_rect_t* rect);
        
        private void get_view_rect(cef_render_handler_t* self, cef_browser_t* browser, cef_rect_t* rect)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.GetViewRect
        }
        
        /// <summary>
        /// Called to retrieve the view rectangle in screen DIP coordinates. This
        /// method must always provide a non-empty rectangle.
        /// </summary>
        // protected abstract void GetViewRect(cef_browser_t* browser, cef_rect_t* rect);
        
        private int get_screen_point(cef_render_handler_t* self, cef_browser_t* browser, int viewX, int viewY, int* screenX, int* screenY)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.GetScreenPoint
        }
        
        /// <summary>
        /// Called to retrieve the translation from view DIP coordinates to screen
        /// coordinates. Windows/Linux should provide screen device (pixel)
        /// coordinates and MacOS should provide screen DIP coordinates. Return true
        /// if the requested coordinates were provided.
        /// </summary>
        // protected abstract int GetScreenPoint(cef_browser_t* browser, int viewX, int viewY, int* screenX, int* screenY);
        
        private int get_screen_info(cef_render_handler_t* self, cef_browser_t* browser, cef_screen_info_t* screen_info)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.GetScreenInfo
        }
        
        /// <summary>
        /// Called to allow the client to fill in the CefScreenInfo object with
        /// appropriate values. Return true if the |screen_info| structure has been
        /// modified.
        /// If the screen info rectangle is left empty the rectangle from GetViewRect
        /// will be used. If the rectangle is still empty or invalid popups may not be
        /// drawn correctly.
        /// </summary>
        // protected abstract int GetScreenInfo(cef_browser_t* browser, cef_screen_info_t* screen_info);
        
        private void on_popup_show(cef_render_handler_t* self, cef_browser_t* browser, int show)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnPopupShow
        }
        
        /// <summary>
        /// Called when the browser wants to show or hide the popup widget. The popup
        /// should be shown if |show| is true and hidden if |show| is false.
        /// </summary>
        // protected abstract void OnPopupShow(cef_browser_t* browser, int show);
        
        private void on_popup_size(cef_render_handler_t* self, cef_browser_t* browser, cef_rect_t* rect)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnPopupSize
        }
        
        /// <summary>
        /// Called when the browser wants to move or resize the popup widget. |rect|
        /// contains the new location and size in view coordinates.
        /// </summary>
        // protected abstract void OnPopupSize(cef_browser_t* browser, cef_rect_t* rect);
        
        private void on_paint(cef_render_handler_t* self, cef_browser_t* browser, CefPaintElementType type, UIntPtr dirtyRectsCount, cef_rect_t* dirtyRects, void* buffer, int width, int height)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnPaint
        }
        
        /// <summary>
        /// Called when an element should be painted. Pixel values passed to this
        /// method are scaled relative to view coordinates based on the value of
        /// CefScreenInfo.device_scale_factor returned from GetScreenInfo. |type|
        /// indicates whether the element is the view or the popup widget. |buffer|
        /// contains the pixel data for the whole image. |dirtyRects| contains the set
        /// of rectangles in pixel coordinates that need to be repainted. |buffer|
        /// will be |width|*|height|*4 bytes in size and represents a BGRA image with
        /// an upper-left origin. This method is only called when
        /// CefWindowInfo::shared_texture_enabled is set to false.
        /// </summary>
        // protected abstract void OnPaint(cef_browser_t* browser, CefPaintElementType type, UIntPtr dirtyRectsCount, cef_rect_t* dirtyRects, void* buffer, int width, int height);
        
        private void on_accelerated_paint(cef_render_handler_t* self, cef_browser_t* browser, CefPaintElementType type, UIntPtr dirtyRectsCount, cef_rect_t* dirtyRects, cef_accelerated_paint_info_t* info)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnAcceleratedPaint
        }
        
        /// <summary>
        /// Called when an element has been rendered to the shared texture handle.
        /// |type| indicates whether the element is the view or the popup widget.
        /// |dirtyRects| contains the set of rectangles in pixel coordinates that need
        /// to be repainted. |info| contains the shared handle; on Windows it is a
        /// HANDLE to a texture that can be opened with D3D11 OpenSharedResource, on
        /// macOS it is an IOSurface pointer that can be opened with Metal or OpenGL,
        /// and on Linux it contains several planes, each with an fd to the underlying
        /// system native buffer.
        /// The underlying implementation uses a pool to deliver frames. As a result,
        /// the handle may differ every frame depending on how many frames are
        /// in-progress. The handle's resource cannot be cached and cannot be accessed
        /// outside of this callback. It should be reopened each time this callback is
        /// executed and the contents should be copied to a texture owned by the
        /// client application. The contents of |info| will be released back to the
        /// pool after this callback returns.
        /// </summary>
        // protected abstract void OnAcceleratedPaint(cef_browser_t* browser, CefPaintElementType type, UIntPtr dirtyRectsCount, cef_rect_t* dirtyRects, cef_accelerated_paint_info_t* info);
        
        private void get_touch_handle_size(cef_render_handler_t* self, cef_browser_t* browser, CefHorizontalAlignment orientation, cef_size_t* size)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.GetTouchHandleSize
        }
        
        /// <summary>
        /// Called to retrieve the size of the touch handle for the specified
        /// |orientation|.
        /// </summary>
        // protected abstract void GetTouchHandleSize(cef_browser_t* browser, CefHorizontalAlignment orientation, cef_size_t* size);
        
        private void on_touch_handle_state_changed(cef_render_handler_t* self, cef_browser_t* browser, cef_touch_handle_state_t* state)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnTouchHandleStateChanged
        }
        
        /// <summary>
        /// Called when touch handle state is updated. The client is responsible for
        /// rendering the touch handles.
        /// </summary>
        // protected abstract void OnTouchHandleStateChanged(cef_browser_t* browser, cef_touch_handle_state_t* state);
        
        private int start_dragging(cef_render_handler_t* self, cef_browser_t* browser, cef_drag_data_t* drag_data, CefDragOperationsMask allowed_ops, int x, int y)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.StartDragging
        }
        
        /// <summary>
        /// Called when the user starts dragging content in the web view. Contextual
        /// information about the dragged content is supplied by |drag_data|.
        /// (|x|, |y|) is the drag start location in screen coordinates.
        /// OS APIs that run a system message loop may be used within the
        /// StartDragging call.
        /// Return false to abort the drag operation. Don't call any of
        /// CefBrowserHost::DragSource*Ended* methods after returning false.
        /// Return true to handle the drag operation. Call
        /// CefBrowserHost::DragSourceEndedAt and DragSourceSystemDragEnded either
        /// synchronously or asynchronously to inform the web view that the drag
        /// operation has ended.
        /// </summary>
        // protected abstract int StartDragging(cef_browser_t* browser, cef_drag_data_t* drag_data, CefDragOperationsMask allowed_ops, int x, int y);
        
        private void update_drag_cursor(cef_render_handler_t* self, cef_browser_t* browser, CefDragOperationsMask operation)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.UpdateDragCursor
        }
        
        /// <summary>
        /// Called when the web view wants to update the mouse cursor during a
        /// drag & drop operation. |operation| describes the allowed operation
        /// (none, move, copy, link).
        /// </summary>
        // protected abstract void UpdateDragCursor(cef_browser_t* browser, CefDragOperationsMask operation);
        
        private void on_scroll_offset_changed(cef_render_handler_t* self, cef_browser_t* browser, double x, double y)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnScrollOffsetChanged
        }
        
        /// <summary>
        /// Called when the scroll offset has changed.
        /// </summary>
        // protected abstract void OnScrollOffsetChanged(cef_browser_t* browser, double x, double y);
        
        private void on_ime_composition_range_changed(cef_render_handler_t* self, cef_browser_t* browser, cef_range_t* selected_range, UIntPtr character_boundsCount, cef_rect_t* character_bounds)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnImeCompositionRangeChanged
        }
        
        /// <summary>
        /// Called when the IME composition range has changed. |selected_range| is the
        /// range of characters that have been selected. |character_bounds| is the
        /// bounds of each character in view coordinates.
        /// </summary>
        // protected abstract void OnImeCompositionRangeChanged(cef_browser_t* browser, cef_range_t* selected_range, UIntPtr character_boundsCount, cef_rect_t* character_bounds);
        
        private void on_text_selection_changed(cef_render_handler_t* self, cef_browser_t* browser, cef_string_t* selected_text, cef_range_t* selected_range)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnTextSelectionChanged
        }
        
        /// <summary>
        /// Called when text selection has changed for the specified |browser|.
        /// |selected_text| is the currently selected text and |selected_range| is
        /// the character range.
        /// </summary>
        // protected abstract void OnTextSelectionChanged(cef_browser_t* browser, cef_string_t* selected_text, cef_range_t* selected_range);
        
        private void on_virtual_keyboard_requested(cef_render_handler_t* self, cef_browser_t* browser, CefTextInputMode input_mode)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderHandler.OnVirtualKeyboardRequested
        }
        
        /// <summary>
        /// Called when an on-screen keyboard should be shown or hidden for the
        /// specified |browser|. |input_mode| specifies what kind of keyboard
        /// should be opened. If |input_mode| is CEF_TEXT_INPUT_MODE_NONE, any
        /// existing keyboard for this browser should be hidden.
        /// </summary>
        // protected abstract void OnVirtualKeyboardRequested(cef_browser_t* browser, CefTextInputMode input_mode);
        
    }
}
