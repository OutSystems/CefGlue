using Avalonia.Input;
using System.Collections.Generic;

namespace Xilium.CefGlue.Avalonia
{
    /// <summary>
    /// Provides handles for mouse cursors.
    /// </summary>
    internal class CursorsProvider
    {
        private static readonly IDictionary<CefCursorType, StandardCursorType> _cefCursorTypeToAvaloniaMap = new Dictionary<CefCursorType, StandardCursorType>() {
            { CefCursorType.Pointer, StandardCursorType.Arrow },
            { CefCursorType.Cross, StandardCursorType.Cross },
            { CefCursorType.Hand, StandardCursorType.Hand },
            { CefCursorType.IBeam, StandardCursorType.Ibeam },
            { CefCursorType.Wait, StandardCursorType.Wait },
            { CefCursorType.Help, StandardCursorType.Help },
            { CefCursorType.EastResize, StandardCursorType.RightSide },
            { CefCursorType.NorthResize, StandardCursorType.TopSide },
            { CefCursorType.NorthEastResize, StandardCursorType.TopRightCorner },
            { CefCursorType.NorthWestResize, StandardCursorType.TopLeftCorner },
            { CefCursorType.SouthResize, StandardCursorType.BottomSide },
            { CefCursorType.SouthEastResize, StandardCursorType.BottomRightCorner },
            { CefCursorType.SouthWestResize, StandardCursorType.BottomLeftCorner },
            { CefCursorType.WestResize, StandardCursorType.LeftSide },
            { CefCursorType.NorthSouthResize, StandardCursorType.SizeNorthSouth },
            { CefCursorType.EastWestResize, StandardCursorType.SizeWestEast },
            { CefCursorType.NorthEastSouthWestResize, StandardCursorType.None },
            { CefCursorType.NorthWestSouthEastResize, StandardCursorType.None },
            { CefCursorType.ColumnResize, StandardCursorType.SizeWestEast },
            { CefCursorType.RowResize, StandardCursorType.SizeNorthSouth },
            { CefCursorType.MiddlePanning, StandardCursorType.None },
            { CefCursorType.EastPanning, StandardCursorType.None },
            { CefCursorType.NorthPanning, StandardCursorType.None },
            { CefCursorType.NorthEastPanning, StandardCursorType.None },
            { CefCursorType.NorthWestPanning, StandardCursorType.None },
            { CefCursorType.SouthPanning, StandardCursorType.None },
            { CefCursorType.SouthEastPanning, StandardCursorType.None },
            { CefCursorType.SouthWestPanning, StandardCursorType.None },
            { CefCursorType.WestPanning, StandardCursorType.None },
            { CefCursorType.Move, StandardCursorType.SizeAll },
            { CefCursorType.VerticalText, StandardCursorType.None },
            { CefCursorType.Cell, StandardCursorType.None },
            { CefCursorType.ContextMenu, StandardCursorType.None },
            { CefCursorType.Alias, StandardCursorType.None },
            { CefCursorType.Progress, StandardCursorType.None },
            { CefCursorType.NoDrop, StandardCursorType.No },
            { CefCursorType.Copy, StandardCursorType.None },
            { CefCursorType.None, StandardCursorType.None },
            { CefCursorType.NotAllowed, StandardCursorType.No },
            { CefCursorType.ZoomIn, StandardCursorType.None },
            { CefCursorType.ZoomOut, StandardCursorType.None },
            { CefCursorType.Grab, StandardCursorType.Hand },
            { CefCursorType.Grabbing, StandardCursorType.Hand },
            { CefCursorType.MiddlePanningVertical, StandardCursorType.None },
            { CefCursorType.MiddlePanningHorizontal, StandardCursorType.None },
            { CefCursorType.Custom, StandardCursorType.None },
            { CefCursorType.DragAndDropNone, StandardCursorType.No },
            { CefCursorType.DragAndDropMove, StandardCursorType.DragMove },
            { CefCursorType.DragAndDropCopy, StandardCursorType.DragCopy },
            { CefCursorType.DragAndDropLink, StandardCursorType.DragLink },
        };

        private static readonly IDictionary<CefDragOperationsMask, StandardCursorType> _cefDragOpsToAvaloniaMap = new Dictionary<CefDragOperationsMask, StandardCursorType>() {
            { CefDragOperationsMask.Copy, StandardCursorType.DragCopy },
            { CefDragOperationsMask.Link, StandardCursorType.DragLink },
            { CefDragOperationsMask.Move, StandardCursorType.DragMove }
        };

        public static Cursor GetCursorFromCefType(CefCursorType cursorType)
        {
            if (_cefCursorTypeToAvaloniaMap.TryGetValue(cursorType, out var avaloniaCursor))
            {
                return new Cursor(avaloniaCursor);
            }
            return Cursor.Default;
        }

        public static Cursor GetCursorFromCefType(CefDragOperationsMask ops)
        {
            if (_cefDragOpsToAvaloniaMap.TryGetValue(ops, out var avaloniaCursor))
            {
                return new Cursor(avaloniaCursor);
            }
            return new Cursor(StandardCursorType.No);
        }
    }
}
