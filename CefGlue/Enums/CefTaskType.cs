//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_task_type_t.
//
namespace Xilium.CefGlue
{
    using System;

    /// <summary>
    /// Specifies the task type variants supported by CefTaskManager.
    /// Should be kept in sync with Chromium's task_manager::Task::Type type.
    /// </summary>
    public enum CefTaskType
    {
        UNKNOWN,
        /// The main browser process.
        BROWSER,
        /// A graphics process.
        GPU,
        /// A Linux zygote process.
        ZYGOTE,
        /// A browser utility process.
        UTILITY,
        /// A normal WebContents renderer process.
        RENDERER,
        /// An extension or app process.
        EXTENSION,
        /// A browser plugin guest process.
        GUEST,
        /// A plugin process.
        PLUGIN,
        /// A sandbox helper process
        SANDBOX_HELPER,
        /// A dedicated worker running on the renderer process.
        DEDICATED_WORKER,
        /// A shared worker running on the renderer process.
        SHARED_WORKER,
        /// A service worker running on the renderer process.
        SERVICE_WORKER,
        NUM_VALUES,
    }
}
