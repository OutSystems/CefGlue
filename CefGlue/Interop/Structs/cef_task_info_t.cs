//
// This file manually written from cef/include/internal/cef_types.h.
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_task_info_t
    {
        /// The task ID.
        public ulong id;
        /// The task type.
        public CefTaskType type;
        /// Set to true (1) if the task is killable.
        public int is_killable;
        /// The task title.
        public cef_string_t title;
        /// The CPU usage of the process on which the task is running. The value is
        /// in the range zero to number_of_processors * 100%.
        public double cpu_usage;
        /// The number of processors available on the system.
        public int number_of_processors;
        /// The memory footprint of the task in bytes. A value of -1 means no valid
        /// value is currently available.
        public ulong memory;
        /// The GPU memory usage of the task in bytes. A value of -1 means no valid
        /// value is currently available.
        public ulong gpu_memory;
        /// Set to true (1) if this task process' GPU resource count is inflated
        /// because it is counting other processes' resources (e.g, the GPU process
        /// has this value set to true because it is the aggregate of all processes).
        public int is_gpu_memory_inflated;
    }
}
