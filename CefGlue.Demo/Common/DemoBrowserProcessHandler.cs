namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    internal sealed class DemoBrowserProcessHandler : CefBrowserProcessHandler
    {
        protected override void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
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

                    var mono = CefRuntime.Platform == CefRuntimePlatform.Linux ? "/usr/bin/mono" : @"C:\Program Files\Mono-2.10.8\bin\monow.exe";
                    commandLine.PrependArgument(mono);

                    commandLine.AppendSwitch("cefglue", "w");
                }
            }

            Console.WriteLine("  -> {0}", commandLine);
        }
    }
}
