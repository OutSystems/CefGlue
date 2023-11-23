# CefGlue
.NET binding for The Chromium Embedded Framework (CEF). 

CefGlue lets you embed Chromium in .NET apps. It is a .NET wrapper control around the Chromium Embedded Framework ([CEF](https://bitbucket.org/chromiumembedded/cef/src/master/)). 
It can be used from C# or any other CLR language and provides both Avalonia and WPF web browser control implementations.
The Avalonia implementation runs on Windows, Linux and macOS.

| Arch     | Windows | macOS | Linux |
| -------- | ------- | ----- | ----- |
| x64      |    x    |   x   |   x   |
| AMD64    |    x    |   x   |   -   |

| Framework | Windows | macOS | Linux |
| --------- | ------- | ----- | ----- |
| Avalonia  |    x    |   x   |   x   |
| WPF       |    x    |   -   |   -   |
 
## Releases
Stable binaries are released on NuGet, and contain everything you need to embed Chromium in your .NET/CLR application. 
- [![CefGlue.Avalonia](https://img.shields.io/nuget/v/CefGlue.Avalonia.svg?style=flat&label=CefGlue-Avalonia)](https://www.nuget.org/packages/CefGlue.Avalonia/)
- [![CefGlue.Avalonia.ARM64](https://img.shields.io/nuget/v/CefGlue.Avalonia.ARM64.svg?style=flat&label=CefGlue-Avalonia-ARM64)](https://www.nuget.org/packages/CefGlue.Avalonia.ARM64/)
- [![CefGlue.Common](https://img.shields.io/nuget/v/CefGlue.Common.svg?style=flat&label=CefGlue-Common)](https://www.nuget.org/packages/CefGlue.Common/)
- [![CefGlue.Common.ARM64](https://img.shields.io/nuget/v/CefGlue.Common.ARM64.svg?style=flat&label=CefGlue-Common-ARM64)](https://www.nuget.org/packages/CefGlue.Common.ARM64/)
- [![CefGlue.WPF](https://img.shields.io/nuget/v/CefGlue.WPF.svg?style=flat&label=CefGlue-WPF)](https://www.nuget.org/packages/CefGlue.WPF/)
- [![CefGlue.WPF.ARM64](https://img.shields.io/nuget/v/CefGlue.WPF.ARM64.svg?style=flat&label=CefGlue-WPF-ARM64)](https://www.nuget.org/packages/CefGlue.WPF.ARM64/)

## Documentation 
See the [Avalonia sample](CefGlue.Demo.Avalonia) or [WPF sample](CefGlue.Demo.WPF) projects for example web browsers built with CefGlue. They demo some of the available features.
