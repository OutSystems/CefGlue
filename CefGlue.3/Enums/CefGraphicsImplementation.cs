namespace Xilium.CefGlue
{
    /// <summary>
    /// Supported graphics implementations.
    /// </summary>
    public enum CefGraphicsImplementation
    {
        Default = 0,
        DefaultCommandBuffer,

        DesktopInProcess,
        DesktopInProcessCommandBuffer,

        // windows only
        AngleInProcess,
        AngleInProcessCommandBuffer,
    }
}
