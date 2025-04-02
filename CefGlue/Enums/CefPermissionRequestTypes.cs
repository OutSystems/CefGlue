//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_permission_request_types_t.
//
namespace Xilium.CefGlue
{
    using System;

    /// <summary>
    /// Permission types used with OnShowPermissionPrompt. Some types are
    /// platform-specific or only supported with Chrome style. Should be kept
    /// in sync with Chromium's permissions::RequestType type.
    /// </summary>
    [Flags]
    public enum CefPermissionRequestTypes
    {
        None = 0,
        ArSession = 1 << 0,
        CameraPanTiltZoom = 1 << 1,
        CameraStream = 1 << 2,
        CapturedSurfaceControl = 1 << 3,
        Clipboard = 1 << 4,
        TopLevelStorageAccess = 1 << 5,
        DiskQuota = 1 << 6,
        LocalFonts = 1 << 7,
        Geolocation = 1 << 8,
        HandTracking = 1 << 9,
        IdentityProvider = 1 << 10,
        IdleDetection = 1 << 11,
        MicStream = 1 << 12,
        MidiSysex = 1 << 13,
        MultipleDownloads = 1 << 14,
        Notifications = 1 << 15,
        KeyboardLock = 1 << 16,
        PointerLock = 1 << 17,
        ProtectedMediaIdentifier = 1 << 18,
        RegisterProtocolHandler = 1 << 19,
        StorageAccess = 1 << 20,
        VrSession = 1 << 21,
        WebAppInstallation = 1 << 22,
        WindowManagement = 1 << 23,
        FileSystemAccess = 1 << 24,
    }
}
