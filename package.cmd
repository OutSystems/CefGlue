dotnet pack CefGlue.Avalonia\Cefglue.Avalonia.csproj -p:Version=%1 -c %2 --no-build -o "Nuget\output"
dotnet pack CefGlue.Common\Cefglue.Common.csproj -p:Version=%1 -c %2 --no-build -o "Nuget\output"
NuGet\nuget pack NuGet\CefGlue.Wpf.nuspec -Version %1 -properties Configuration=%2 -OutputDirectory "Nuget\output"
