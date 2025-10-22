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
        Unknown,
        /// The main browser process.
        Browser,
        /// A graphics process.
        Gpu,
        /// A Linux zygote process.
        Zygote,
        /// A browser utility process.
        Utility,
        /// A normal WebContents renderer process.
        Renderer,
        /// An extension or app process.
        Extension,
        /// A browser plugin guest process.
        Guest,
        /// A plugin process.
        Plugin,
        /// A sandbox helper process
        SandboxHelper,
        /// A dedicated worker running on the renderer process.
        DedicatedWorker,
        /// A shared worker running on the renderer process.
        SharedWorker,
        /// A service worker running on the renderer process.
        ServiceWorker,
        NumValues,
    }
}
