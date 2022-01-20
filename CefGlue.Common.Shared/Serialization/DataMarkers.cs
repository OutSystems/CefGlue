namespace Xilium.CefGlue.Common.Shared.Serialization
{
    public class DataMarkers
    {
        public const int MarkerLength = 1;
        
        // special data markers used to distinguish the several string types
        public const string DateTimeMarker = "D";
        public const string StringMarker = "S";
        public const string BinaryMarker = "B";
    }
}
