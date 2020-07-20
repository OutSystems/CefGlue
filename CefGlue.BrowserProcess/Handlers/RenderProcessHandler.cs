using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xilium.CefGlue.BrowserProcess.JavascriptExecution;
using Xilium.CefGlue.BrowserProcess.ObjectBinding;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.Handlers
{
    internal class RenderProcessHandler : CefRenderProcessHandler
    {
        private CefBrowser _browser;
        private JavascriptExecutionEngineRenderSide _javascriptExecutionEngine;
        private JavascriptToNativeDispatcherRenderSide _javascriptToNativeDispatcher;

        private readonly MessageDispatcher _messageDispatcher = new MessageDispatcher();

        public RenderProcessHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        protected override void OnWebKitInitialized()
        {
            base.OnWebKitInitialized();
            _javascriptExecutionEngine = new JavascriptExecutionEngineRenderSide(_messageDispatcher);
            _javascriptToNativeDispatcher = new JavascriptToNativeDispatcherRenderSide(_messageDispatcher);
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            WithErrorHandling(() =>
            {
                using (CefObjectTracker.StartTracking())
                {
                    _messageDispatcher.DispatchMessage(browser, frame, sourceProcess, message);
                }
            }, frame);
            return base.OnProcessMessageReceived(browser, frame, sourceProcess, message);
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            WithErrorHandling(() =>
            {
                using (CefObjectTracker.StartTracking())
                {
                    base.OnContextCreated(browser, frame, context);
                    _javascriptToNativeDispatcher.HandleContextCreated(context, frame.IsMain);

                    var message = new Messages.JsContextCreated();
                    var cefMessage = message.ToCefProcessMessage();
                    frame.SendProcessMessage(CefProcessId.Browser, cefMessage);
                }
            }, frame);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            WithErrorHandling(() =>
            {
                using (CefObjectTracker.StartTracking())
                {
                    _javascriptToNativeDispatcher.HandleContextReleased(context, frame.IsMain);
                    base.OnContextReleased(browser, frame, context);

                    var message = new Messages.JsContextReleased();
                    var cefMessage = message.ToCefProcessMessage();
                    frame.SendProcessMessage(CefProcessId.Browser, cefMessage);
                }
            }, frame);
        }

        protected override void OnUncaughtException(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
        {
            WithErrorHandling(() =>
            {
                using (CefObjectTracker.StartTracking())
                {
                    var frames = new Messages.JsStackFrame[stackTrace.FrameCount];
                    for (var i = 0; i < stackTrace.FrameCount; i++)
                    {
                        var stackFrame = stackTrace.GetFrame(i);
                        frames[i] = new Messages.JsStackFrame()
                        {
                            FunctionName = stackFrame.FunctionName,
                            ScriptNameOrSourceUrl = stackFrame.ScriptNameOrSourceUrl,
                            LineNumber = stackFrame.LineNumber,
                            Column = stackFrame.Column
                        };
                    }

                    var message = new Messages.JsUncaughtException()
                    {
                        Message = exception.Message,
                        StackFrames = frames
                    };

                    var cefMessage = message.ToCefProcessMessage();
                    frame.SendProcessMessage(CefProcessId.Browser, cefMessage);

                    base.OnUncaughtException(browser, frame, context, exception, stackTrace);
                }
            }, frame);
        }


        protected override void OnBrowserCreated(CefBrowser browser, CefDictionaryValue extraInfo)
        {
            _browser = browser;
            base.OnBrowserCreated(browser, extraInfo);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            LogException(exception, _browser?.FrameCount > 0 ? _browser?.GetMainFrame() : null);
        }

        private void WithErrorHandling(Action action, CefFrame frame)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                LogException(e, frame);
            }
        }

        private void LogException(Exception e, CefFrame frame)
        {
            try
            {
                if (frame != null)
                {
                    using (CefObjectTracker.StartTracking())
                    {
                        var exceptionMessage = new Messages.UnhandledException()
                        {
                            ExceptionType = e.GetType().FullName,
                            Message = e.Message,
                            StackTrace = e.StackTrace
                        };
                        var message = exceptionMessage.ToCefProcessMessage();
                        frame.SendProcessMessage(CefProcessId.Browser, message);
                    }
                }
                else
                {
                    LogExceptionToFile(e);
                }
            }
            catch
            {
                LogExceptionToFile(e);
            }

        }

        private void LogExceptionToFile(Exception e)
        {
            const string LogFileArg = "--log-file=";
            
            var args = Environment.GetCommandLineArgs();
            var logFile = args.FirstOrDefault(a => a.StartsWith(LogFileArg));
            if (string.IsNullOrEmpty(logFile)) {
                // no logfile arg
                return;
            }

            logFile = logFile.Substring(LogFileArg.Length).Trim('"');
            if (string.IsNullOrEmpty(logFile)) {
                // no logfile path
                return;
            }

            try {
                var logPath = new DirectoryInfo(Path.GetDirectoryName(logFile));
                if (!logPath.Exists)
                {
                    logPath.Create();
                }
                var processLogFile = Path.Combine(logPath.FullName, Path.GetFileNameWithoutExtension(logFile) + "-" + Process.GetCurrentProcess().Id + ".cefrenderlog");
                File.WriteAllText(processLogFile, e.ToString());
            } catch {
                // failed at logging
            }
        }
    }
}
