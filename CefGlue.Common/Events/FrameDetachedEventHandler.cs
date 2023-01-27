using System;

namespace Xilium.CefGlue.Common.Events;

public delegate void FrameDetachedEventHandler(object sender, FrameDetachedEventArgs e);

public class FrameDetachedEventArgs : EventArgs
{
    public FrameDetachedEventArgs(CefFrame frame)
    {
        Frame = frame;
    }

    public CefFrame Frame { get; }
}