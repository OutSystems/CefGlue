namespace Xilium.CefGlue
{
    public interface ICefBinaryValue
    {
        bool IsOwned { get; }
        bool IsValid { get; }
        long Size { get; }

        ICefBinaryValue Copy();
        void Dispose();
        long GetData(byte[] buffer, long bufferSize, long dataOffset);
        bool IsEqual(ICefBinaryValue that);
        bool IsSame(ICefBinaryValue that);
        byte[] ToArray();
    }
}