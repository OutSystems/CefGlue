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
        MidiSysex = 1 << 11,
        MultipleDownloads = 1 << 12,
        Notifications = 1 << 13,
        ProtectedMediaIdentifier = 1 << 14,
        RegisterProtocolHandler = 1 << 15,
        SecurityAttestation = 1 << 16,
        StorageAccess = 1 << 17,
        U2fApiRequest = 1 << 18,
        VrSession = 1 << 19,
        WindowManagement = 1 << 20,
    }
}
