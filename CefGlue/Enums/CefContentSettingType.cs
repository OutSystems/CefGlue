using System;
using System.Collections.Generic;
using System.Text;

namespace Xilium.CefGlue
{
    /// <summary>
    /// Supported content setting types. Some types are platform-specific or only
    /// supported with the Chrome runtime. Should be kept in sync with Chromium's
    /// ContentSettingsType type. 
    /// </summary>
    public enum CefContentSettingType
    {
        // This setting governs whether cookies are enabled by the user in the
        /// provided context. However, it may be overridden by other settings. This
        /// enum should NOT be read directly to determine whether cookies are enabled;
        /// the client should instead rely on the CookieSettings API.
        Cookies = 0,
        Images,
        JavaScript,

        /// This setting governs both popups and unwanted redirects like tab-unders
        /// and framebusting.
        Popups,

        Geolocation,
        Notifications,
        AutoSelectCertificate,
        MixedScript,
        MediaStreamMic,
        MediaStreamCamera,
        ProtocolHandlers,
        DeprecatedPpapiBroker,
        AutomaticDownloads,

        /// Advanced device-specific functions on MIDI devices. MIDI-SysEx
        /// communications can be used for changing the MIDI device's persistent state
        /// such as firmware.
        MidiSysEx,

        SslCertDecisions,
        ProtectedMediaIdentifier,
        AppBanner,
        SiteEngagement,
        DurableStorage,
        UsbChooserData,
        BluetoothGuard,
        BackgroundSync,
        Autoplay,
        ImportantSiteInfo,
        PermissionAutoBlockerData,
        Ads,

        /// Website setting which stores metadata for the subresource filter to aid in
        /// decisions for whether or not to show the UI.
        AdsData,

        /// MIDI stands for Musical Instrument Digital Interface. It is a standard
        /// that allows electronic musical instruments, computers, and other devices
        /// to communicate with each other.
        Midi,

        /// This content setting type is for caching password protection service's
        /// verdicts of each origin.
        PasswordProtection,

        /// Website setting which stores engagement data for media related to a
        /// specific origin.
        MediaEngagement,

        /// Content setting which stores whether or not the site can play audible
        /// sound. This will not block playback but instead the user will not hear it.
        Sound,

        /// Website setting which stores the list of client hints that the origin
        /// requested the browser to remember. The browser is expected to send all
        /// client hints in the HTTP request headers for every resource requested
        /// from that origin.
        ClientHints,

        /// Generic Sensor API covering ambient-light-sensor, accelerometer, gyroscope
        /// and magnetometer are all mapped to a single content_settings_type.
        /// Setting for the Generic Sensor API covering ambient-light-sensor,
        /// accelerometer, gyroscope and magnetometer. These are all mapped to a
        /// single ContentSettingsType.
        Sensors,

        /// Content setting which stores whether or not the user has granted the site
        /// permission to respond to accessibility events, which can be used to
        /// provide a custom accessibility experience. Requires explicit user consent
        /// because some users may not want sites to know they're using assistive
        /// technology.
        AccessibilityEvents,

        /// Used to store whether to allow a website to install a payment handler.
        PaymentHandler,

        /// Content setting which stores whether to allow sites to ask for permission
        /// to access USB devices. If this is allowed specific device permissions are
        /// stored under USB_CHOOSER_DATA.
        UsbGuard,

        /// Nothing is stored in this setting at present. Please refer to
        /// BackgroundFetchPermissionContext for details on how this permission
        /// is ascertained.
        BackgroundFetch,

        /// Website setting which stores the amount of times the user has dismissed
        /// intent picker UI without explicitly choosing an option.
        IntentPickerDisplay,

        /// Used to store whether to allow a website to detect user active/idle state.
        IdleDetection,

        /// Content settings for access to serial ports. The "guard" content setting
        /// stores whether to allow sites to ask for permission to access a port. The
        /// permissions granted to access particular ports are stored in the "chooser
        /// data" website setting.
        SerialGuard,
        SerialChooserData,

        /// Nothing is stored in this setting at present. Please refer to
        /// PeriodicBackgroundSyncPermissionContext for details on how this permission
        /// is ascertained.
        /// This content setting is not registered because it does not require access
        /// to any existing providers.
        PeriodicBackgroundSync,

        /// Content setting which stores whether to allow sites to ask for permission
        /// to do Bluetooth scanning.
        BluetoothScanning,

        /// Content settings for access to HID devices. The "guard" content setting
        /// stores whether to allow sites to ask for permission to access a device.
        /// The permissions granted to access particular devices are stored in the
        /// "chooser data" website setting.
        HidGuard,
        HidChooserData,

        /// Wake Lock API, which has two lock types: screen and system locks.
        /// Currently, screen locks do not need any additional permission, and system
        /// locks are always denied while the right UI is worked out.
        WakeLockScreen,
        WakeLockSystem,

        /// Legacy SameSite cookie behavior. This disables SameSite=Lax-by-default,
        /// SameSite=None requires Secure, and Schemeful Same-Site, forcing the
        /// legacy behavior wherein 1) cookies that don't specify SameSite are treated
        /// as SameSite=None, 2) SameSite=None cookies are not required to be Secure,
        /// and 3) schemeful same-site is not active.
        ///
        /// This will also be used to revert to legacy behavior when future changes
        /// in cookie handling are introduced.
        LegacyCookieAccess,

        /// Content settings which stores whether to allow sites to ask for permission
        /// to save changes to an original file selected by the user through the
        /// File System Access API.
        FileSystemWriteGuard,

        /// Used to store whether to allow a website to exchange data with NFC
        /// devices.
        Nfc,

        /// Website setting to store permissions granted to access particular
        /// Bluetooth devices.
        BluetoothChooserData,

        /// Full access to the system clipboard (sanitized read without user gesture,
        /// and unsanitized read and write with user gesture).
        ClipboardReadWrite,

        /// This is special-cased in the permissions layer to always allow, and as
        /// such doesn't have associated prefs data.
        ClipboardSanitizedWrite,

        /// This content setting type is for caching safe browsing real time url
        /// check's verdicts of each origin.
        SafeBrowsingUrlCheckData,

        /// Used to store whether a site is allowed to request AR or VR sessions with
        /// the WebXr Device API.
        VR,
        AR,

        /// Content setting which stores whether to allow site to open and read files
        /// and directories selected through the File System Access API.
        FileSystemReadGuard,

        /// Access to first party storage in a third-party context. Exceptions are
        /// scoped to the combination of requesting/top-level origin, and are managed
        /// through the Storage Access API. For the time being, this content setting
        /// exists in parallel to third-party cookie rules stored in COOKIES.
        StorageAccess,

        /// Content setting which stores whether to allow a site to control camera
        /// movements. It does not give access to camera.
        CameraPanTiltZoom,

        /// Content setting for Screen Enumeration and Screen Detail functionality.
        /// Permits access to detailed multi-screen information, like size and
        /// position. Permits placing fullscreen and windowed content on specific
        /// screens. See also: https://w3c.github.io/window-placement
        WindowManagement,

        /// Stores whether to allow insecure websites to make private network
        /// requests.
        /// See also: https://wicg.github.io/cors-rfc1918
        /// Set through enterprise policies only.
        InsecurePrivateNetwork,

        /// Content setting which stores whether or not a site can access low-level
        /// locally installed font data using the Local Fonts Access API.
        LocalFonts,

        /// Stores per-origin state for permission auto-revocation (for all permission
        /// types).
        PermissionAutoRevocationData,

        /// Stores per-origin state of the most recently selected directory for the
        /// use by the File System Access API.
        FileSystemLastPickedDirectory,

        /// Controls access to the getDisplayMedia API when {preferCurrentTab: true}
        /// is specified.
        DisplayCapture,

        /// Website setting to store permissions metadata granted to paths on the
        /// local file system via the File System Access API.
        /// |FILE_SYSTEM_WRITE_GUARD| is the corresponding "guard" setting.
        FileSystemAccessChooserData,

        /// Stores a grant that allows a relying party to send a request for identity
        /// information to specified identity providers, potentially through any
        /// anti-tracking measures that would otherwise prevent it. This setting is
        /// associated with the relying party's origin.
        FederatedIdentitySharing,

        /// Whether to use the v8 optimized JIT for running JavaScript on the page.
        JavaScriptJit,

        /// Content setting which stores user decisions to allow loading a site over
        /// HTTP. Entries are added by hostname when a user bypasses the HTTPS-First
        /// Mode interstitial warning when a site does not support HTTPS. Allowed
        /// hosts are exact hostname matches -- subdomains of a host on the allowlist
        /// must be separately allowlisted.
        HttpAllowed,

        /// Stores metadata related to form fill, such as e.g. whether user data was
        /// autofilled on a specific website.
        FormFillMetadata,

        /// Setting to indicate that there is an active federated sign-in session
        /// between a specified relying party and a specified identity provider for
        /// a specified account. When this is present it allows access to session
        /// management capabilities between the sites. This setting is associated
        /// with the relying party's origin.
        FederatedIdentityActiveSession,

        /// Setting to indicate whether Chrome should automatically apply darkening to
        /// web content.
        AutoDarkWebContent,

        /// Setting to indicate whether Chrome should request the desktop view of a
        /// site instead of the mobile one.
        RequestDesktopSite,

        /// Setting to indicate whether browser should allow signing into a website
        /// via the browser FedCM API.
        FederatedIdentityApi,

        /// Stores notification interactions per origin for the past 90 days.
        /// Interactions per origin are pre-aggregated over seven-day windows: A
        /// notification interaction or display is assigned to the last Monday
        /// midnight in local time.
        NotificationInteractions,

        /// Website setting which stores the last reduced accept language negotiated
        /// for a given origin, to be used on future visits to the origin.
        ReducedAcceptLanguage,

        /// Website setting which is used for NotificationPermissionReviewService to
        /// store origin blocklist from review notification permissions feature.
        NotificationPermissionReview,

        /// Website setting to store permissions granted to access particular devices
        /// in private network.
        PrivateNetworkGuard,
        PrivateNetworkChooserData,

        /// Website setting which stores whether the browser has observed the user
        /// signing into an identity-provider based on observing the IdP-SignIn-Status
        /// HTTP header.
        FederatedIdentityIdentityProviderSignInStatus,

        /// Website setting which is used for UnusedSitePermissionsService to
        /// store revoked permissions of unused sites from unused site permissions
        /// feature.
        RevokedUnusedSitePermissions,

        /// Similar to STORAGE_ACCESS, but applicable at the page-level rather than
        /// being specific to a frame.
        TopLevelStorageAccess,

        /// Setting to indicate whether user has opted in to allowing auto re-authn
        /// via the FedCM API.
        FederatedIdentityAutoReAuthNPermission,

        /// Website setting which stores whether the user has explicitly registered
        /// a website as an identity-provider.
        FederatedIdentityIdentityProviderRegistration,

        /// Content setting which is used to indicate whether anti-abuse functionality
        /// should be enabled.
        AntiAbuse,

        /// Content setting used to indicate whether third-party storage partitioning
        /// should be enabled.
        ThirdPartyStoragePartitioning,

        /// Used to indicate whether HTTPS-First Mode is enabled on the hostname.
        HttpsEnforced,

        /// Setting for enabling the `getAllScreensMedia` API. Spec link:
        /// https://github.com/screen-share/capture-all-screens
        AllScreenCapture,

        /// Stores per origin metadata for cookie controls.
        CookieControlsMetadata,

        /// Content Setting for 3PC accesses granted via 3PC deprecation trial.
        TpcdSupport,

        /// Content setting used to indicate whether entering picture-in-picture
        /// automatically should be enabled.
        AutoPictureInPicture,

        /// Content Setting for 3PC accesses granted by metadata delivered via the
        /// component updater service. This type will only be used when
        /// `net::features::kTpcdMetadataGrants` is enabled.
        TpcdMetadataGrants,

        // Whether user has opted into keeping file/directory permissions persistent
        /// between visits for a given origin. When enabled, permission metadata
        /// stored under |FILE_SYSTEM_ACCESS_CHOOSER_DATA| can auto-grant incoming
        /// permission request.
        FileSystemAccessExtendedPermission,

        /// Content Setting for temporary 3PC accesses granted by user behavior
        /// heuristics.
        TpcdHeuristicsGrants,

        NumTypes,
    }
}
