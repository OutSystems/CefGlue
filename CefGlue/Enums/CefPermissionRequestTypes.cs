//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_permission_request_types_t.
//
namespace Xilium.CefGlue
{
    using System;

    /// <summary>
    /// Permission types used with OnShowPermissionPrompt. Some types are
    /// platform-specific or only supported with the Chrome runtime. Should be kept
    /// in sync with Chromium's permissions::RequestType type.
    /// </summary>
    [Flags]
    public enum CefPermissionRequestTypes
    {
        None = 0,
        AccessibilityEvents = 1 << 0,
        ArSession = 1 << 1,
        CameraPanTiltZoom = 1 << 2,
        CameraStream = 1 << 3,
        Clipboard = 1 << 4,
        TopLevelStorageAccess = 1 << 5,
        DiskQuota = 1 << 6,
        LocalFonts = 1 << 7,
        Geolocation = 1 << 8,
        IdleDetection = 1 << 9,
        MicStream = 1 << 10,
        Midi = 1 << 11,
        MidiSysex = 1 << 12,
        MultipleDownloads = 1 << 13,
        Notifications = 1 << 14,
        ProtectedMediaIdentifier = 1 << 15,
        RegisterProtocolHandler = 1 << 16,
        StorageAccess = 1 << 17,
        VrSession = 1 << 18,
        WindowManagement = 1 << 19,
        FileSystemAccess = 1 << 20,
    }
}
