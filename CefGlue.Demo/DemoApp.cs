namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class DemoApp : IDisposable
    {
        private IMainView _mainView;

        protected DemoApp()
        {
        }

        #region IDisposable

        ~DemoApp()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion

        public string Name { get { return "Xilium CefGlue Demo"; } }
        public int DefaultWidth { get { return 800; } }
        public int DefaultHeight { get { return 600; } }
        public string HomeUrl { get { return "http://google.com"; } }

        protected IMainView MainView { get { return _mainView; } }

        public int Run(string[] args)
        {
            CefRuntime.Load();

            var settings = new CefSettings();
            settings.MultiThreadedMessageLoop = CefRuntime.Platform == CefRuntimePlatform.Windows;
            settings.LogSeverity = CefLogSeverity.Verbose;
            settings.LogFile = "cef.log";
            settings.ResourcesDirPath = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath);

            var argv = args;
            if (CefRuntime.Platform != CefRuntimePlatform.Windows)
            {
                argv = new string[args.Length + 1];
                Array.Copy(args, 0, argv, 1, args.Length);
                argv[0] = "-";
            }

            var mainArgs = new CefMainArgs(argv);
            var app = new DemoCefApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);
            Console.WriteLine("CefRuntime.ExecuteProcess() returns {0}", exitCode);
            if (exitCode != -1)
                return exitCode;

            // guard if something wrong
            foreach (var arg in args) { if (arg.StartsWith("--type=")) { return -2; } }

            CefRuntime.Initialize(mainArgs, settings, app);

            PlatformInitialize();

            var mainMenu = CreateMainMenu();
            _mainView = CreateMainView(mainMenu);
            _mainView.NewTab(HomeUrl);

            PlatformRunMessageLoop();

            _mainView.Dispose();
            _mainView = null;

            CefRuntime.Shutdown();

            PlatformShutdown();
            return 0;
        }

        public void Quit()
        {
            PlatformQuitMessageLoop();
        }

        private MenuItem[] CreateMainMenu()
        {
            return new[] {
                new MenuItem("File", new [] {
                    new MenuItem(new Command("New Tab...", FileNewTabCommand)),
                    new MenuItem(new Command("Exit", FileExitCommand)),
                    }),
                new MenuItem("Samples", new [] {
                    new MenuItem("Hello, world #1"),
                    new MenuItem("Hello, world #2"),
                    }),
                new MenuItem("Help", new [] {
                    new MenuItem(new Command("About", HelpAboutCommand)),
                    }),
            };
        }

        protected abstract void PlatformInitialize();

        protected abstract void PlatformShutdown();

        protected abstract void PlatformRunMessageLoop();

        protected abstract void PlatformQuitMessageLoop();

        protected abstract IMainView CreateMainView(MenuItem[] menu);

        #region Commands

        private void FileNewTabCommand(object sender, EventArgs e)
        {
            MainView.NewTab(HomeUrl);
        }

        private void FileExitCommand(object sender, EventArgs e)
        {
            MainView.Close();
        }

        private void HelpAboutCommand(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
