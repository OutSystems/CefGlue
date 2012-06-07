//
// This file manually written from cef/include/internal/cef_types.h.
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_browser_settings_t
    {
        public UIntPtr size;
        public bool_t drag_drop_disabled;
        public bool_t load_drops_disabled;
        public bool_t history_disabled;
        public cef_string_t standard_font_family;
        public cef_string_t fixed_font_family;
        public cef_string_t serif_font_family;
        public cef_string_t sans_serif_font_family;
        public cef_string_t cursive_font_family;
        public cef_string_t fantasy_font_family;
        public int default_font_size;
        public int default_fixed_font_size;
        public int minimum_font_size;
        public int minimum_logical_font_size;
        public bool_t remote_fonts_disabled;
        public cef_string_t default_encoding;
        public bool_t encoding_detector_enabled;
        public bool_t javascript_disabled;
        public bool_t javascript_open_windows_disallowed;
        public bool_t javascript_close_windows_disallowed;
        public bool_t javascript_access_clipboard_disallowed;
        public bool_t dom_paste_disabled;
        public bool_t caret_browsing_enabled;
        public bool_t java_disabled;
        public bool_t plugins_disabled;
        public bool_t universal_access_from_file_urls_allowed;
        public bool_t file_access_from_file_urls_allowed;
        public bool_t web_security_disabled;
        public bool_t xss_auditor_enabled;
        public bool_t image_load_disabled;
        public bool_t shrink_standalone_images_to_fit;
        public bool_t site_specific_quirks_disabled;
        public bool_t text_area_resize_disabled;
        public bool_t page_cache_disabled;
        public bool_t tab_to_links_disabled;
        public bool_t hyperlink_auditing_disabled;
        public bool_t user_style_sheet_enabled;
        public cef_string_t user_style_sheet_location;
        public bool_t author_and_user_styles_disabled;
        public bool_t local_storage_disabled;
        public bool_t databases_disabled;
        public bool_t application_cache_disabled;
        public bool_t webgl_disabled;
        public bool_t accelerated_compositing_disabled;
        public bool_t accelerated_layers_disabled;
        public bool_t accelerated_video_disabled;
        public bool_t accelerated_2d_canvas_disabled;
        public bool_t accelerated_painting_enabled;
        public bool_t accelerated_filters_enabled;
        public bool_t accelerated_plugins_disabled;
        public bool_t developer_tools_disabled;
        public bool_t fullscreen_enabled;

        #region Alloc & Free
        private static int _sizeof;

        static cef_browser_settings_t()
        {
            _sizeof = Marshal.SizeOf(typeof(cef_browser_settings_t));
        }

        public static cef_browser_settings_t* Alloc()
        {
            var ptr = (cef_browser_settings_t*)Marshal.AllocHGlobal(_sizeof);
            *ptr = new cef_browser_settings_t();
            ptr->size = (UIntPtr)_sizeof;
            return ptr;
        }

        public static void Free(cef_browser_settings_t* ptr)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
        #endregion
    }
}
