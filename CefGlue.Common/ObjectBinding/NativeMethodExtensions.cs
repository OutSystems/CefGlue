using System.Linq;

using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding;

internal static class NativeMethodExtensions
{
    public static MethodInfo ToMethodInfo(this NativeMethod method)
        => new(method.Name.ToCamelCase(), method.MandatoryParameterCount);

    public static ObjectInfo ToObjectInfo(this NativeObject nativeObject)
        => new(nativeObject.Name.ToCamelCase(), nativeObject.Methods.Select(ToMethodInfo).ToArray());

    public static string ToCamelCase(this string name)
        => name.Length > 0 ? name.Substring(0, 1).ToLowerInvariant() + name.Substring(1) : name;
}