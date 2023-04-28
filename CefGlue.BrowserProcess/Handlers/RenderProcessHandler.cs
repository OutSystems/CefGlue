using System;
using Xilium.CefGlue.BrowserProcess.JavascriptExecution;
using Xilium.CefGlue.BrowserProcess.ObjectBinding;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.Handlers
{
    internal class RenderProcessHandler : CefRenderProcessHandler
    {
        private CefBrowser _browser;
        private string _crashPipeName;
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
            using (message)
            
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
            _crashPipeName = extraInfo.GetString(Constants.CrashPipeNameKey);
            _browser = browser;
            base.OnBrowserCreated(browser, extraInfo);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CefFrame frame = null;
            var exception = (Exception)e.ExceptionObject;
            try
            {
                frame = _browser?.FrameCount > 0 ? _browser?.GetMainFrame() : null;
            } 
            catch
            {
                // ignore
            }
            HandleException(exception, frame);
        }

        private void WithErrorHandling(Action action, CefFrame frame)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                HandleException(e, frame);
            }
        }

        private void HandleException(Exception e, CefFrame frame)
        {
            if (frame != null)
            {
                try
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
                    return;
                }
                catch
                {
                    // ignore, lets try an alternative method using the crash pipe
                }
            }
            SendExceptionToParentProcess(e);
        }

        /// <summary>
        /// Alternative way to send the exception to the parent process using a side named pipe.
        /// </summary>
        /// <param name="e"></param>
        private void SendExceptionToParentProcess(Exception e)
        {
            try
            {
                if (string.IsNullOrEmpty(_crashPipeName))
                {
                    return; // not initialized yet
                }

                var serializableException = new SerializableException()
                {
                    ExceptionType = e.GetType().ToString(),
                    Message = e.Message,
                    StackTrace = e.StackTrace
                };

                
                using (var pipeClient = new PipeClient(_crashPipeName))
                {
                    pipeClient.SendMessage(serializableException.SerializeToString());
                }
            }
            catch
            {
                // failed at failing
            }
        }
    }
}
