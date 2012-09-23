namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.IO;

    internal sealed class DemoCefApp : CefApp
    {
        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            Console.WriteLine("OnBeforeCommandLineProcessing: {0} {1}", processType, commandLine);

            // TODO: currently on linux platform location of locales and pack files are determined
            // incorrectly (relative to main module instead of libcef.so module).
            // Once issue http://code.google.com/p/chromiumembedded/issues/detail?id=668 will be resolved
            // this code can be removed.
            if (CefRuntime.Platform == CefRuntimePlatform.Linux)
            {
                var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
                path = Path.GetDirectoryName(path);

                commandLine.AppendSwitch("resources-dir-path", path);
                commandLine.AppendSwitch("locales-dir-path", Path.Combine(path, "locales"));
            }
        }

        protected override void AppendExtraCommandLineSwitches(CefCommandLine commandLine)
        {
            Console.WriteLine("AppendExtraCommandLineSwitches: {0}", commandLine);
            Console.WriteLine(" Program == {0}", commandLine.GetProgram());

            // .NET in Windows treat assemblies as native images, so no any magic required.
            // Mono on any platform usually located far away from entry assembly, so we want prepare command line to call it correctly.
            if (Type.GetType("Mono.Runtime") != null)
            {
                if (!commandLine.HasSwitch("cefglue"))
                {
                    var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
                    commandLine.SetProgram(path);

                    commandLine.PrependWrapper("-");

                    var mono = CefRuntime.Platform == CefRuntimePlatform.Linux ? "/usr/bin/mono" : @"C:\Program Files\Mono-2.10.8\bin\monow.exe";
                    commandLine.SetProgram(mono);

                    commandLine.AppendSwitch("cefglue", "w");
                }
            }

            Console.WriteLine("  -> {0}", commandLine);
        }
    }
}
