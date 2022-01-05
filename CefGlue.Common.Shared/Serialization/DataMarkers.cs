namespace Xilium.CefGlue.Common.Shared.Serialization
{
    public class DataMarkers
    {
        // special data markers used to distinguish the several string types
        public const string DateTimeMarker = "#d#";
        public const string StringMarker = "#s#";
        
        public enum BinaryMagicBytes : byte
        {
            DateTime,
            Binary
        }
    }
}
