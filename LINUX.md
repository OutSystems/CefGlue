# Linux

While porting CefGlue to Linux we tested on these platforms and confirmed it to be working:

 - Arch Linux (rolling release, packages up-to-date as on 29 July 2024) on x64
 - Debian 12 on x64
 - Kylin V10 on ARM64 (see issues below)

## ARM64 Issues

We have not found the issue why dynamic loading CEF is failing with "cannot allocate memory in static TLS block" and currently our guess is that CLR uses too much TLS. 
Loading CEf with `LD_PRELOAD` environment variable works but needs to load HarfBuzzSharp first (`LD_PRELOAD=/path/to/libHarfBuzzSharp.so:/path/to/libcef.so`).

One can also modify the ELF files using these commands before running their CefGlue application for CEF to load correctly:
````bash
patchelf --add-needed libHarfBuzzSharp.so --add-needed libcef.so path/to/Xilium.CefGlue.Demo.Avalonia
patchelf --add-needed libcef.so path/to/Xilium.CefGlue.BrowserProcess
# Browser process doesn't use Avalonia, so there's no need to add libHarfBuzzSharp.so
````
