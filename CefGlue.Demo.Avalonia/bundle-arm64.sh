#!/bin/sh

dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-arm64 -p:Platform=ARM64

TARGETAPP=bin/ARM64/Debug/net6.0/osx-arm64/publish/CefGlueDemoAvalonia.app/Contents/MacOS
chmod +x "$TARGETAPP/CefGlueBrowserProcess/Xilium.CefGlue.BrowserProcess"
chmod +x "$TARGETAPP/Xilium.CefGlue.Demo.Avalonia"
