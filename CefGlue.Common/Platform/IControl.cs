using System;
using System.Collections.Generic;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    internal interface IControl
    {
        event Action GotFocus;

        event Action<CefSize> SizeChanged;

        IntPtr? GetHostViewHandle(int initialWidth, int initialHeight);

        void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback);
        void CloseContextMenu();

        void SetTooltip(string text);

        void InitializeRender(IntPtr browserHandle);

        void DestroyRender();

        bool SetCursor(IntPtr cursorHandle, CefCursorType cursorType);
    }
}