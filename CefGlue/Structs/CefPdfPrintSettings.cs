namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue.Interop;

    [Serializable]
    public sealed class CefPdfPrintSettings
    {
        public string HeaderFooterTitle { get; set; }
        public string HeaderFooterUrl { get; set; }
        public int PageWidth { get; set; }
        public int PageHeight { get; set; }
        public double MarginTop { get; set; }
        public double MarginRight { get; set; }
        public double MarginBottom { get; set; }
        public double MarginLeft { get; set; }
        public CefPdfPrintMarginType MarginType { get; set; }
        public bool HeaderFooterEnabled { get; set; }
        public bool SelectionOnly { get; set; }
        public bool Landscape { get; set; }
        public bool BackgroundsEnabled { get; set; }

        internal unsafe cef_pdf_print_settings_t* ToNative()
        {
            var ptr = cef_pdf_print_settings_t.Alloc();

            cef_string_t.Copy(HeaderFooterTitle, &ptr->header_footer_title);
            cef_string_t.Copy(HeaderFooterUrl, &ptr->header_footer_url);
            ptr->page_width = PageWidth;
            ptr->page_height = PageHeight;
            ptr->margin_top = MarginTop;
            ptr->margin_right = MarginRight;
            ptr->margin_bottom = MarginBottom;
            ptr->margin_left = MarginLeft;
            ptr->margin_type = MarginType;
            ptr->header_footer_enabled = HeaderFooterEnabled ? 1 : 0;
            ptr->selection_only = SelectionOnly ? 1 : 0;
            ptr->landscape = Landscape ? 1 : 0;
            ptr->backgrounds_enabled = BackgroundsEnabled ? 1 : 0;

            return ptr;
        }
    }
}
