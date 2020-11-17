NuGet\nuget pack NuGet\CefGlue.Common.nuspec -Version %1 -Properties "RedistVersion=84.3.10" -OutputDirectory "Nuget\output"
NuGet\nuget pack NuGet\CefGlue.Wpf.nuspec -Version %1 -OutputDirectory "Nuget\output"
NuGet\nuget pack NuGet\CefGlue.Avalonia.nuspec -Version %1 -OutputDirectory "Nuget\output"
