namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    //
    // While C# bool type is not blittable, we use this struct to represent C++ bool type.
    //

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN, Size = 1)]
    internal unsafe struct bool_t
    {
        private byte _value;

        public bool_t(bool value)
        {
            _value = (byte)(value ? 1 : 0);
        }

        public static implicit operator bool(bool_t value)
        {
            return value._value != 0;
        }

        public static implicit operator bool_t(bool value)
        {
            return new bool_t(value);
        }
    }
}
