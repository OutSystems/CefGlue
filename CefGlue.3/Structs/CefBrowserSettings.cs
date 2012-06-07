namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue.Interop;

    public sealed unsafe class CefBrowserSettings
    {
        private cef_browser_settings_t* _self;

        public CefBrowserSettings()
        {
            _self = cef_browser_settings_t.Alloc();
        }

        internal CefBrowserSettings(cef_browser_settings_t* ptr)
        {
            _self = ptr;
        }

        internal void Dispose()
        {
            _self = null;
        }

        /// <summary>
        /// Disable drag & drop of URLs from other windows.
        /// </summary>
        public bool DragDropDisabled
        {
            get { return _self->drag_drop_disabled; }
            set { _self->drag_drop_disabled = value; }
        }

        /// <summary>
        /// Disable default navigation resulting from drag & drop of URLs.
        /// </summary>
        public bool LoadDropsDisabled
        {
            get { return _self->load_drops_disabled; }
            set { _self->load_drops_disabled = value; }
        }

        /// <summary>
        /// Disable history back/forward navigation.
        /// </summary>
        public bool HistoryDisabled
        {
            get { return _self->history_disabled; }
            set { _self->history_disabled = value; }
        }


        /// The below values map to WebPreferences settings.

        /// <summary>
        /// Font settings.
        /// </summary>
        public string StandardFontFamily
        {
            get { return cef_string_t.ToString(&_self->standard_font_family); }
            set { cef_string_t.Copy(value, &_self->standard_font_family); }
        }

        public string FixedFontFamily
        {
            get { return cef_string_t.ToString(&_self->fixed_font_family); }
            set { cef_string_t.Copy(value, &_self->fixed_font_family); }
        }

        public string SerifFontFamily
        {
            get { return cef_string_t.ToString(&_self->serif_font_family); }
            set { cef_string_t.Copy(value, &_self->serif_font_family); }
        }

        public string SansSerifFontFamily
        {
            get { return cef_string_t.ToString(&_self->sans_serif_font_family); }
            set { cef_string_t.Copy(value, &_self->sans_serif_font_family); }
        }

        public string CursiveFontFamily
        {
            get { return cef_string_t.ToString(&_self->cursive_font_family); }
            set { cef_string_t.Copy(value, &_self->cursive_font_family); }
        }

        public string FantasyFontFamily
        {
            get { return cef_string_t.ToString(&_self->fantasy_font_family); }
            set { cef_string_t.Copy(value, &_self->fantasy_font_family); }
        }

        public int DefaultFontSize
        {
            get { return _self->default_font_size; }
            set { _self->default_font_size = value; }
        }

        public int DefaultFixedFontSize
        {
            get { return _self->default_fixed_font_size; }
            set { _self->default_fixed_font_size = value; }
        }

        public int MinimumFontSize
        {
            get { return _self->minimum_font_size; }
            set { _self->minimum_font_size = value; }
        }

        public int MinimumLogicalFontSize
        {
            get { return _self->minimum_logical_font_size; }
            set { _self->minimum_logical_font_size = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable loading of fonts from remote sources.
        /// </summary>
        public bool RemoteFontsDisabled
        {
            get { return _self->remote_fonts_disabled; }
            set { _self->remote_fonts_disabled = value;}
        }

        /// <summary>
        /// Default encoding for Web content. If empty "ISO-8859-1" will be used.
        /// </summary>
        public string DefaultEncoding
        {
            get { return cef_string_t.ToString(&_self->default_encoding); }
            set { cef_string_t.Copy(value, &_self->default_encoding); }
        }

        /// <summary>
        /// Set to <c>true</c> to attempt automatic detection of content encoding.
        /// </summary>
        public bool EncodingDetectorEnabled
        {
            get { return _self->encoding_detector_enabled; }
            set { _self->encoding_detector_enabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable JavaScript.
        /// </summary>
        public bool JavaScriptDisabled
        {
            get { return _self->javascript_disabled; }
            set { _self->javascript_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disallow JavaScript from opening windows.
        /// </summary>
        public bool JavaScriptOpenWindowsDisallowed
        {
            get { return _self->javascript_open_windows_disallowed; }
            set { _self->javascript_open_windows_disallowed = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disallow JavaScript from closing windows.
        /// </summary>
        public bool JavaScriptCloseWindowsDisallowed
        {
            get { return _self->javascript_close_windows_disallowed; }
            set { _self->javascript_close_windows_disallowed = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disallow JavaScript from accessing the clipboard.
        /// </summary>
        public bool JavaScriptAccessClipboardDisallowed
        {
            get { return _self->javascript_access_clipboard_disallowed; }
            set { _self->javascript_access_clipboard_disallowed = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable DOM pasting in the editor. DOM pasting also
        /// depends on |javascript_cannot_access_clipboard| being <c>false</c>.
        /// </summary>
        public bool DomPasteDisabled
        {
            get { return _self->dom_paste_disabled; }
            set { _self->dom_paste_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to enable drawing of the caret position.
        /// </summary>
        public bool CaretBrowsingEnabled
        {
            get { return _self->caret_browsing_enabled; }
            set { _self->caret_browsing_enabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable Java.
        /// </summary>
        public bool JavaDisabled
        {
            get { return _self->java_disabled; }
            set { _self->java_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable plugins.
        /// </summary>
        public bool PluginsDisabled
        {
            get { return _self->plugins_disabled; }
            set { _self->plugins_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to allow access to all URLs from file URLs.
        /// </summary>
        public bool UniversalAccessFromFileUrlsAllowed
        {
            get { return _self->universal_access_from_file_urls_allowed; }
            set { _self->universal_access_from_file_urls_allowed = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to allow access to file URLs from other file URLs.
        /// </summary>
        public bool FileAccessFromFileUrlsAllowed
        {
            get { return _self->file_access_from_file_urls_allowed; }
            set { _self->file_access_from_file_urls_allowed = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to allow risky security behavior such as cross-site
        /// scripting (XSS). Use with extreme care.
        /// </summary>
        public bool WebSecurityDisabled
        {
            get { return _self->web_security_disabled; }
            set { _self->web_security_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to enable console warnings about XSS attempts.
        /// </summary>
        public bool XssAuditorEnabled
        {
            get { return _self->xss_auditor_enabled; }
            set { _self->xss_auditor_enabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to suppress the network load of image URLs.  A cached
        /// image will still be rendered if requested.
        /// </summary>
        public bool ImageLoadDisabled
        {
            get { return _self->image_load_disabled; }
            set { _self->image_load_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to shrink standalone images to fit the page.
        /// </summary>
        public bool ShrinkStandaloneImagesToFit
        {
            get { return _self->shrink_standalone_images_to_fit; }
            set { _self->shrink_standalone_images_to_fit = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable browser backwards compatibility features.
        /// </summary>
        public bool SiteSpecificQuirksDisabled
        {
            get { return _self->site_specific_quirks_disabled; }
            set { _self->site_specific_quirks_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable resize of text areas.
        /// </summary>
        public bool TextAreaResizeDisabled
        {
            get { return _self->text_area_resize_disabled; }
            set { _self->text_area_resize_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable use of the page cache.
        /// </summary>
        public bool PageCacheDisabled
        {
            get { return _self->page_cache_disabled; }
            set { _self->page_cache_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to not have the tab key advance focus to links.
        /// </summary>
        public bool TabToLinksDisabled
        {
            get { return _self->tab_to_links_disabled; }
            set { _self->tab_to_links_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable hyperlink pings (<a ping> and window.sendPing).
        /// </summary>
        public bool HyperlinkAuditingDisabled
        {
            get { return _self->hyperlink_auditing_disabled; }
            set { _self->hyperlink_auditing_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to enable the user style sheet for all pages.
        /// </summary>
        public bool UserStyleSheetEnabled
        {
            get { return _self->user_style_sheet_enabled; }
            set { _self->user_style_sheet_enabled = value; }
        }

        /// <summary>
        /// Location of the user style sheet. This must be a data URL of the form
        /// "data:text/css { get; set; }charset=utf-8 { get; set; }base64,csscontent" where "csscontent" is the
        /// base64 encoded contents of the CSS file.
        /// </summary>
        public string UserStyleSheetLocation
        {
            get { return cef_string_t.ToString(&_self->user_style_sheet_location); }
            set { cef_string_t.Copy(value, &_self->user_style_sheet_location); }
        }

        /// <summary>
        /// Set to <c>true</c> to disable style sheets.
        /// </summary>
        public bool AuthorAndUserStylesDisabled
        {
            get { return _self->author_and_user_styles_disabled; }
            set { _self->author_and_user_styles_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable local storage.
        /// </summary>
        public bool LocalStorageDisabled
        {
            get { return _self->local_storage_disabled; }
            set { _self->local_storage_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable databases.
        /// </summary>
        public bool DatabasesDisabled
        {
            get { return _self->databases_disabled; }
            set { _self->databases_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable application cache.
        /// </summary>
        public bool ApplicationCacheDisabled
        {
            get { return _self->application_cache_disabled; }
            set { _self->application_cache_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable WebGL.
        /// </summary>
        public bool WebGLDisabled
        {
            get { return _self->webgl_disabled; }
            set { _self->webgl_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable accelerated compositing.
        /// </summary>
        public bool AcceleratedCompositingDisabled
        {
            get { return _self->accelerated_compositing_disabled; }
            set { _self->accelerated_compositing_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable accelerated layers. This affects features like
        /// 3D CSS transforms.
        /// </summary>
        public bool AcceleratedLayersDisabled
        {
            get { return _self->accelerated_layers_disabled; }
            set { _self->accelerated_layers_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable accelerated video.
        /// </summary>
        public bool AcceleratedVideoDisabled
        {
            get { return _self->accelerated_video_disabled; }
            set { _self->accelerated_video_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable accelerated 2d canvas.
        /// </summary>
        public bool Accelerated2DCanvasDisabled
        {
            get { return _self->accelerated_2d_canvas_disabled; }
            set { _self->accelerated_2d_canvas_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to enable accelerated painting.
        /// </summary>
        public bool AcceleratedPaintingEnabled
        {
            get { return _self->accelerated_painting_enabled; }
            set { _self->accelerated_painting_enabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to enable accelerated filters.
        /// </summary>
        public bool AcceleratedFiltersEnabled
        {
            get { return _self->accelerated_filters_enabled; }
            set { _self->accelerated_filters_enabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable accelerated plugins.
        /// </summary>
        public bool AcceleratedPluginsDisabled
        {
            get { return _self->accelerated_plugins_disabled; }
            set { _self->accelerated_plugins_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to disable developer tools (WebKit inspector).
        /// </summary>
        public bool DeveloperToolsDisabled
        {
            get { return _self->developer_tools_disabled; }
            set { _self->developer_tools_disabled = value; }
        }

        /// <summary>
        /// Set to <c>true</c> to enable fullscreen mode.
        /// </summary>
        public bool FullscreenEnabled
        {
            get { return _self->fullscreen_enabled; }
            set { _self->fullscreen_enabled = value; }
        }


        internal cef_browser_settings_t* ToNative()
        {
            return _self;
        }
    }
}
