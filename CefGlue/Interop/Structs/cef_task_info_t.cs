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
        public UIntPtr size;
        public ulong id;
        public CefTaskType type;
        public int is_killable;
        public cef_string_t title;
        public double cpu_usage;
        public int number_of_processors;
        public ulong memory;
        public ulong gpu_memory;
        public int is_gpu_memory_inflated;
    }
}
