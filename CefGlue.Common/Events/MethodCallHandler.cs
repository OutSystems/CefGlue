using System;

namespace Xilium.CefGlue.Common.Events
{
    public delegate object MethodCallHandler(Func<object> originalFunction);
}
