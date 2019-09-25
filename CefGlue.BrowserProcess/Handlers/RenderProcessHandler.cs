using System;
using Xilium.CefGlue.BrowserProcess.JavascriptExecution;
using Xilium.CefGlue.BrowserProcess.ObjectBinding;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

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

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            WithErrorHandling(() =>
            {
                _messageDispatcher.DispatchMessage(browser, sourceProcess, message);
            });
            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            WithErrorHandling(() =>
            {
                base.OnContextCreated(browser, frame, context);
                _javascriptToNativeDispatcher.HandleContextCreated(context, frame.IsMain);

                var message = new Messages.JsContextCreated();
                message.FrameId = frame.Name;
                browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());
            });
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            WithErrorHandling(() =>
            {
                _javascriptToNativeDispatcher.HandleContextReleased(context, frame.IsMain);
                base.OnContextReleased(browser, frame, context);

                var message = new Messages.JsContextReleased();
                message.FrameId = frame.Name;
                browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());
            });
        }

        protected override void OnUncaughtException(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
        {
            WithErrorHandling(() =>
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

                var message = new Messages.JsUncaughtException();
                message.FrameId = frame.Name;
                message.Message = exception.Message;
                message.StackFrames = frames;
                browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());

                base.OnUncaughtException(browser, frame, context, exception, stackTrace);
            });
        }

        protected override void OnBrowserCreated(CefBrowser browser)
        {
            _browser = browser;
            base.OnBrowserCreated(browser);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WithErrorHandling(() => throw (Exception) e.ExceptionObject);
        }

        private void WithErrorHandling(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                try
                {
                    if (_browser != null)
                    {
                        var exceptionMessage = new Messages.UnhandledException()
                        {
                            ExceptionType = e.GetType().FullName,
                            Message = e.Message,
                            StackTrace = e.StackTrace
                        };
                        using (var message = exceptionMessage.ToCefProcessMessage())
                        {
                            _browser.SendProcessMessage(CefProcessId.Browser, message);
                        }
                    }
                    else
                    {
                        // TODO log to file?
                    }
                }
                catch
                {
                    // TODO what if we fail at failing?
                }
            }
        }
    }
}
