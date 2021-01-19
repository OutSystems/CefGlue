#!/bin/sh

dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64

TARGETAPP=bin/Debug/net5.0/osx-x64/publish/CefGlueDemoAvalonia.app/Contents/MacOS
chmod +x "$TARGETAPP/CefGlueBrowserProcess/Xilium.CefGlue.BrowserProcess"
chmod +x "$TARGETAPP/Xilium.CefGlue.Demo.Avalonia"