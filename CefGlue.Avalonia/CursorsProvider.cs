using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Avalonia.Input;

namespace Xilium.CefGlue.Avalonia
{
    /// <summary>
    /// Provides handles for mouse cursors.
    /// </summary>
    internal class CursorsProvider
    {
        private static IDictionary<IntPtr, Cursor> _cache = new ConcurrentDictionary<IntPtr, Cursor>();

        static CursorsProvider()
        {
            foreach(StandardCursorType type in Enum.GetValues(typeof(StandardCursorType)))
            {
                var cursor = new Cursor(type);
                _cache[cursor.PlatformCursor.Handle] = cursor;
            }
        }

        public static Cursor GetCursorFromHandle(IntPtr handle)
        {
            if (_cache.TryGetValue(handle, out var cursor))
            {
                return cursor;
            }
            return Cursor.Default;
        }
    }
}
