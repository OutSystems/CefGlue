//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_channel_layout_t.
//

namespace Xilium.CefGlue
{
    /// <summary>
    /// Enumerates the various representations of the ordering of audio channels.
    /// </summary>
    public enum CefChannelLayout
    {
        None,
        Unsupported,

        /// <summary>
        /// Front C
        /// </summary>
        Mono,

        /// <summary>
        /// Front L, Front R
        /// </summary>
        Stereo,

        /// <summary>
        /// Front L, Front R, Back C
        /// </summary>
        Layout_2_1,

        /// <summary>
        /// Front L, Front R, Front C
        /// </summary>
        Surround,

        /// <summary>
        /// Front L, Front R, Front C, Back C
        /// </summary>
        Layout_4_0,

        /// <summary>
        /// Front L, Front R, Side L, Side R
        /// </summary>
        Layout_2_2,

        /// <summary>
        /// Front L, Front R, Back L, Back R
        /// </summary>
        Quad,

        /// <summary>
        /// Front L, Front R, Front C, Side L, Side R
        /// </summary>
        Layout_5_0,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Side L, Side R
        /// </summary>
        Layout_5_1,

        /// <summary>
        /// Front L, Front R, Front C, Back L, Back R
        /// </summary>
        Layout_5_0_Back,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Back L, Back R
        /// </summary>
        Layout_5_1_Back,

        /// <summary>
        /// Front L, Front R, Front C, Back L, Back R, Side L, Side R
        /// </summary>
        Layout_7_0,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Back L, Back R, Side L, Side R
        /// </summary>
        Layout_7_1,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Front LofC, Front RofC, Side L, Side R
        /// </summary>
        Layout_7_1_Wide,

        /// <summary>
        /// Front L, Front R
        /// </summary>
        StereoDownmix,

        /// <summary>
        /// Front L, Front R, LFE
        /// </summary>
        Layout_2Point1,

        /// <summary>
        /// Front L, Front R, Front C, LFE
        /// </summary>
        Layout_3_1,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Back C
        /// </summary>
        Layout_4_1,

        /// <summary>
        /// Front L, Front R, Front C, Back C, Side L, Side R
        /// </summary>
        Layout_6_0,

        /// <summary>
        /// Front L, Front R, Front LofC, Front RofC, Side L, Side R
        /// </summary>
        Layout_6_0_Front,

        /// <summary>
        /// Front L, Front R, Front C, Back L, Back R, Back C
        /// </summary>
        Hexagonal,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Back C, Side L, Side R
        /// </summary>
        Layout_6_1,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Back L, Back R, Back C
        /// </summary>
        Layout_6_1_Back,

        /// <summary>
        /// Front L, Front R, LFE, Front LofC, Front RofC, Side L, Side R
        /// </summary>
        Layout_6_1_Front,

        /// <summary>
        /// Front L, Front R, Front C, Front LofC, Front RofC, Side L, Side R
        /// </summary>
        Layout_7_0_Front,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Back L, Back R, Front LofC, Front RofC
        /// </summary>
        Layout_7_1_WideBack,

        /// <summary>
        /// Front L, Front R, Front C, Back L, Back R, Back C, Side L, Side R
        /// </summary>
        Octagonal,

        /// <summary>
        /// Channels are not explicitly mapped to speakers.
        /// </summary>
        Discrete,

        /// <summary>
        /// Deprecated, but keeping the enum value for UMA consistency.
        /// Front L, Front R, Front C. Front C contains the keyboard mic audio. This
        /// layout is only intended for input for WebRTC. The Front C channel
        /// is stripped away in the WebRTC audio input pipeline and never seen outside
        /// of that.
        /// </summary>
        StereoAndKeyboardMic,

        /// <summary>
        /// Front L, Front R, LFE, Side L, Side R
        /// </summary>
        Layout_4_1_QuadSide,

        /// <summary>
        /// Actual channel layout is specified in the bitstream and the actual channel
        /// count is unknown at Chromium media pipeline level (useful for audio
        /// pass-through mode).
        /// </summary>
        Bitstream,

        /// <summary>
        /// Front L, Front R, Front C, LFE, Side L, Side R,
        /// Front Height L, Front Height R, Rear Height L, Rear Height R
        /// Will be represented as six channels (5.1) due to eight channel limit
        /// kMaxConcurrentChannels
        /// </summary>
        Layout_5_1_4_Downmix,

        /// <summary>
        /// Front C, LFE
        /// </summary>
        Layout_1_1,

        /// <summary>
        /// Front L, Front R, LFE, Back C
        /// </summary>
        Layout_3_1_Back,

        NumValues,
    }
}
