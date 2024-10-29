namespace Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

public static class CefListExtensions
{
    public static bool SetNullableBinary(this ICefListValue valueList, int index, byte[] bytes)
    {
        if (bytes != null && bytes.Length > 0)
        {
            return valueList.SetBinary(index, CefBinaryValue.Create(bytes));
        }

        return valueList.SetNull(index);
    }

    public static byte[] GetNullableBinary(this ICefListValue valueList, int index)
    {
        CefValueType valueType = valueList.GetValueType(index);
        return valueType == CefValueType.Null
            ? []
            : valueList.GetBinary(index).ToArray();
    }

    public static bool SetNullableString(this ICefListValue valueList, int index, string @string)
    {
        if (!string.IsNullOrEmpty(@string))
        {
            return valueList.SetString(index, @string);
        }

        return valueList.SetNull(index);
    }

    public static string GetNullableString(this ICefListValue valueList, int index)
    {
        CefValueType valueType = valueList.GetValueType(index);
        return valueType == CefValueType.Null
            ? default
            : valueList.GetString(index);
    }
}
