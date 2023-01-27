using System;

namespace Xilium.CefGlue.Common.Events;

public delegate void FrameAttachedEventHandler(object sender, FrameAttachedEventArgs e);

public class FrameAttachedEventArgs : EventArgs
{
    public FrameAttachedEventArgs(CefFrame frame, bool reattached)
    {
        Frame = frame;
        Reattached = reattached;
    }

    public CefFrame Frame { get; }

    public bool Reattached { get; }
}