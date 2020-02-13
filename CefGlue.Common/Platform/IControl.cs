using System;
using System.Collections.Generic;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    internal interface IControl : IDisposable
    {
        event Action<CefSize> SizeChanged;

        IntPtr? GetHostViewHandle();

        void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback);
        void CloseContextMenu();

        void SetTooltip(string text);

        void InitializeRender(IntPtr browserHandle);
    }
}