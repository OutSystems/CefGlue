namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_key_event_t
    {
        public CefKeyEventType type;
        public CefKeyEventModifiers modifiers;
        public int windows_key_code;
        public int native_key_code;
        public bool_t is_system_key;
        public char character;
        public char unmodified_character;
        public bool_t focus_on_editable_field;
    }
}
