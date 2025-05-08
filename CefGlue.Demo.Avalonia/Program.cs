using System;
using System.IO;
using System.Threading.Tasks;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Demo.Avalonia
{
    class Program
    {

        static int Main(string[] args)
        {
            CefRuntimeLoader.Initialize();
            Task.Delay(20000).Wait();
            return 0;
        }
    }
}
