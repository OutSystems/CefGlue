# CefGlue
.NET binding for The Chromium Embedded Framework (CEF). 

CefGlue lets you embed Chromium in .NET apps. It is a .NET wrapper control around the Chromium Embedded Framework ([CEF](https://bitbucket.org/chromiumembedded/cef/src/master/)). 
It can be used from C# or any other CLR language and provides both Avalonia and WPF web browser control implementations.
The Avalonia implementation runs on Windows and macOS. Linux is not supported yet.

Currently only x64 architecture is supported.

## Releases
Stable binaries are released on NuGet, and contain everything you need to embed Chromium in your .NET/CLR application. 
- [![CefGlue.Avalonia](https://img.shields.io/nuget/v/CefGlue.Avalonia.svg?style=flat&label=CefGlue-Avalonia)](https://www.nuget.org/packages/CefGlue.Avalonia/)
- [![CefGlue.WPF](https://img.shields.io/nuget/v/CefGlue.WPF.svg?style=flat&label=CefGlue-WPF)](https://www.nuget.org/packages/CefGlue.WPF/)

## Documentation 
See the [Avalonia sample](CefGlue.Demo.Avalonia) or [WPF sample](CefGlue.Demo.WPF) projects for example web browsers built with CefGlue. They demo some of the available features.
