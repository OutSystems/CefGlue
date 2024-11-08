namespace Xilium.CefGlue.Common.Shared.Serialization;

public interface ISerializer
{
    byte[] Serialize(object value);
}