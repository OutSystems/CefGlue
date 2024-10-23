namespace Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

public record ObjectInfo(string Name, MethodInfo[] Methods);

public record struct MethodInfo(string Name, int ParameterCount);