namespace Xilium.CefGlue
{
    public enum CefDragOperationsMask
    {
        DragOperationNone = 0,
        DragOperationCopy = 1,
        DragOperationLink = 2,
        DragOperationGenericC = 4,
        DragOperationPrivate = 8,
        DragOperationMove = 16,
        DragOperationDelete = 32,
        DragOperationEvery = int.MaxValue

    }
}
