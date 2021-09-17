namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Diagnostics;
    //using Xilium.CefGlue.Wrapper;
    using System.Threading;

    public abstract class DemoApp : IDisposable
    {
        //public static CefMessageRouterBrowserSide BrowserMessageRouter { get; private set; }

        private const string DumpRequestDomain = "dump-request.demoapp.cefglue.xilium.local";

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
            try
            {
                return RunInternal(args);
            }
            catch (Exception ex)
            {
                PlatformMessageBox(ex.ToString());
                return 1;
            }
        }

        protected bool MultiThreadedMessageLoop { get; private set; }

        private int RunInternal(string[] args)
        {
            CefRuntime.Load();

            var settings = new CefSettings();
            settings.MultiThreadedMessageLoop = MultiThreadedMessageLoop = CefRuntime.Platform == CefRuntimePlatform.Windows;
            settings.LogSeverity = CefLogSeverity.Default;
            settings.LogFile = "cef.log";
            settings.ResourcesDirPath = System.IO.Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetEntryAssembly().CodeBase).LocalPath);
            settings.RemoteDebuggingPort = 20480;
            settings.NoSandbox = true;

            var argv = args;
            if (CefRuntime.Platform != CefRuntimePlatform.Windows)
            {
                argv = new string[args.Length + 1];
                Array.Copy(args, 0, argv, 1, args.Length);
                argv[0] = "-";
            }

            var mainArgs = new CefMainArgs(argv);
            var app = new DemoCefApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
            Console.WriteLine("CefRuntime.ExecuteProcess() returns {0}", exitCode);
            if (exitCode != -1)
                return exitCode;

            // guard if something wrong
            foreach (var arg in args) { if (arg.StartsWith("--type=")) { return -2; } }

            CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);

            RegisterSchemes();
            RegisterMessageRouter();

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
                    new MenuItem(new Command("Scheme Handler: Dump Request", SchemeHandlerDumpRequestCommand)),
                    new MenuItem(new Command("Send Process Message", SendProcessMessageCommand)),
                    new MenuItem(new Command("Popup Window", PopupWindowCommand)),
                    new MenuItem(new Command("Transparent Popup Window", TransparentPopupWindowCommand)),
                    new MenuItem(new Command("Open Developer Tools...", OpenDeveloperToolsCommand)),
                    new MenuItem(new Command("SendKeyEvent", SendKeyEventCommand)),
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

        protected abstract void PlatformMessageBox(string message);

        #region Commands

        private void FileNewTabCommand(object sender, EventArgs e)
        {
            MainView.NewTab(HomeUrl);
        }

        private void FileExitCommand(object sender, EventArgs e)
        {
            MainView.Close();
        }

        private void SchemeHandlerDumpRequestCommand(object sender, EventArgs e)
        {
            MainView.NavigateTo("http://" + DumpRequestDomain);
        }

        private void SendProcessMessageCommand(object sender, EventArgs e)
        {
            var browser = MainView.CurrentBrowser;
            if (browser != null)
            {
                var message = CefProcessMessage.Create("myMessage1");
                var arguments = message.Arguments;
                arguments.SetString(0, "hello");
                arguments.SetInt(1, 12345);
                arguments.SetDouble(2, 12345.6789);
                arguments.SetBool(3, true);

                browser.GetMainFrame().SendProcessMessage(CefProcessId.Renderer, message);
            }
        }

        private void PopupWindowCommand(object sender, EventArgs e)
        {
            var url = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(new Uri(typeof(DemoApp).Assembly.CodeBase).LocalPath), "transparency.html");
            MainView.NewWebView(url, false);
        }

        private void TransparentPopupWindowCommand(object sender, EventArgs e)
        {
            var url = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(new Uri(typeof(DemoApp).Assembly.CodeBase).LocalPath), "transparency.html");
            MainView.NewWebView(url, true);
        }

        private void OpenDeveloperToolsCommand(object sender, EventArgs e)
        {
            var host = MainView.CurrentBrowser.GetHost();
            var wi = CefWindowInfo.Create();
            wi.SetAsPopup(IntPtr.Zero, "DevTools");
            host.ShowDevTools(wi, new DevToolsWebClient(), new CefBrowserSettings(), new CefPoint(0, 0));
        }

        private class DevToolsWebClient : CefClient
        {
        }

        private void SendKeyEventCommand(object sender, EventArgs e)
        {
            var host = MainView.CurrentBrowser.GetHost();

            foreach (var c in "This text typed with CefBrowserHost.SendKeyEvent method!")
            {
                // little hacky
                host.SendKeyEvent(new CefKeyEvent
                    {
                        EventType = CefKeyEventType.Char,
                        Modifiers= CefEventFlags.None,
                        WindowsKeyCode = c,
                        NativeKeyCode = c,
                        Character = c,
                        UnmodifiedCharacter = c,
                    });
            }
        }

        private void HelpAboutCommand(object sender, EventArgs e)
        {
            // throw new NotImplementedException();
        }

        #endregion

        private void RegisterSchemes()
        {
            // register custom scheme handler
            CefRuntime.RegisterSchemeHandlerFactory("http", DumpRequestDomain, new DemoAppSchemeHandlerFactory());
            // CefRuntime.AddCrossOriginWhitelistEntry("http://localhost", "http", "", true);

        }

        private void RegisterMessageRouter()
        {
            if (!CefRuntime.CurrentlyOn(CefThreadId.UI))
            {
                PostTask(CefThreadId.UI, this.RegisterMessageRouter);
                return;
            }

            // window.cefQuery({ request: 'my_request', onSuccess: function(response) { console.log(response); }, onFailure: function(err,msg) { console.log(err, msg); } });
            //DemoApp.BrowserMessageRouter = new CefMessageRouterBrowserSide(new CefMessageRouterConfig());
            //DemoApp.BrowserMessageRouter.AddHandler(new DemoMessageRouterHandler());
        }

        //private class DemoMessageRouterHandler : CefMessageRouterBrowserSide.Handler
        //{
        //    public override bool OnQuery(CefBrowser browser, CefFrame frame, long queryId, string request, bool persistent, CefMessageRouterBrowserSide.Callback callback)
        //    {
        //        if (request == "wait5")
        //        {
        //            new Thread(() =>
        //            {
        //                Thread.Sleep(5000);
        //                callback.Success("success! responded after 5 sec timeout."); // TODO: at this place crash can occurs, if application closed
        //            }).Start();
        //            return true;
        //        }

        //        if (request == "wait5f")
        //        {
        //            new Thread(() =>
        //            {
        //                Thread.Sleep(5000);
        //                callback.Failure(12345, "success! responded after 5 sec timeout. responded as failure.");
        //            }).Start();
        //            return true;
        //        }

        //        if (request == "wait30")
        //        {
        //            new Thread(() =>
        //            {
        //                Thread.Sleep(30000);
        //                callback.Success("success! responded after 30 sec timeout.");
        //            }).Start();
        //            return true;
        //        }

        //        if (request == "noanswer")
        //        {
        //            return true;
        //        }

        //        var chars = request.ToCharArray();
        //        Array.Reverse(chars);
        //        var response = new string(chars);
        //        callback.Success(response);
        //        return true;
        //    }

        //    public override void OnQueryCanceled(CefBrowser browser, CefFrame frame, long queryId)
        //    {
        //    }
        //}

        public static void PostTask(CefThreadId threadId, Action action)
        {
            CefRuntime.PostTask(threadId, new ActionTask(action));
        }

        internal sealed class ActionTask : CefTask
        {
            public Action _action;

            public ActionTask(Action action)
            {
                _action = action;
            }

            protected override void Execute()
            {
                _action();
                _action = null;
            }
        }

        public delegate void Action();
    }
}
