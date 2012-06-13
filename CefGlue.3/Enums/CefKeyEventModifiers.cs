namespace Xilium.CefGlue
{
    using System;

    /// <summary>
    /// Key event modifiers.
    /// </summary>
    [Flags]
    public enum CefKeyEventModifiers
    {
        Shift = 1 << 0,
        Ctrl = 1 << 1,
        Alt = 1 << 2,
        Meta = 1 << 3,

        /// <summary>
        /// Only used on Mac OS-X.
        /// </summary>
        KeyPad = 1 << 4,
    }
}
