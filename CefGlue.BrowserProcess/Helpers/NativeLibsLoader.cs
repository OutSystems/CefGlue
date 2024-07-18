using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Xilium.CefGlue.BrowserProcess.Helpers
{
    internal static class NativeLibsLoader
    {
        /// <summary>
        /// Installs a native lib loader for loading cef native libs from their right location.
        /// </summary>
        public static void Install()
        {
#if NET5_0_OR_GREATER
            string extension = null;
            var basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."));

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.MacOS:
                    extension = "dylib";
                    break;

                case CefRuntimePlatform.Windows:
                    extension = "dll";
                    break;
                
                case CefRuntimePlatform.Linux:
                    extension = "so";
                    break;
            }

            AssemblyLoadContext.Default.ResolvingUnmanagedDll += (_, libName) =>
            {
                var libPath = Path.Combine(basePath, libName + "." + extension);
                return NativeLibrary.Load(libPath);
            };
#endif
        }

    }
}
