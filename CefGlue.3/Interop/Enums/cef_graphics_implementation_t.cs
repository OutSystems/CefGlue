//
// This file manually written from:
//   cef/include/internal/cef_types_win.h
//   cef/include/internal/cef_types_linux.h
//   cef/include/internal/cef_types_mac.h
//
// C API name: cef_graphics_implementation_t.
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal enum cef_graphics_implementation_t_windows
    {
        ANGLE_IN_PROCESS = 0,
        ANGLE_IN_PROCESS_COMMAND_BUFFER,
        DESKTOP_IN_PROCESS,
        DESKTOP_IN_PROCESS_COMMAND_BUFFER,
    }

    internal enum cef_graphics_implementation_t_posix
    {
        DESKTOP_IN_PROCESS = 0,
        DESKTOP_IN_PROCESS_COMMAND_BUFFER,
    };

}
