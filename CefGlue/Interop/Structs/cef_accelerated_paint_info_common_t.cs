//
// This file manually written from cef/include/internal/cef_types.h.
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_accelerated_paint_info_common_t
    {
        public UIntPtr size;
        
        ///
        /// Timestamp of the frame in microseconds since capture start.
        ///
        public ulong timestamp;

        ///
        /// The full dimensions of the video frame.
        ///
        public cef_size_t coded_size;

        ///
        /// The visible area of the video frame.
        ///
        public cef_rect_t visible_rect;

        ///
        /// The region of the video frame that capturer would like to populate.
        ///
        public cef_rect_t content_rect;

        ///
        /// Full size of the source frame.
        ///
        public cef_size_t source_size;

        ///
        /// Updated area of frame, can be considered as the `dirty` area.
        ///
        public cef_rect_t capture_update_rect;

        ///
        /// May reflects where the frame's contents originate from if region
        /// capture is used internally.
        ///
        public cef_rect_t region_capture_rect;

        ///
        /// The increamental counter of the frame.
        ///
        public ulong capture_counter;

        ///
        /// Optional flag of capture_update_rect
        ///
        public byte has_capture_update_rect;

        ///
        /// Optional flag of region_capture_rect
        ///
        public byte has_region_capture_rect;

        ///
        /// Optional flag of source_size
        ///
        public byte has_source_size;

        ///
        /// Optional flag of capture_counter
        ///
        public byte has_capture_counter;
    }
}
