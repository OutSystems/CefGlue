using System;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;
using Xilium.CefGlue.Common.Handlers;

namespace Xilium.CefGlue.Avalonia
{
    internal class AvaloniaBrowserProcessHandler : BrowserProcessHandler
    {
        private IDisposable _current;
        private object _schedule = new object();

        protected override void OnScheduleMessagePumpWork(long delayMs)
        {
            lock (_schedule)
            {
                if (_current != null)
                {
                    _current.Dispose();
                }

                if (delayMs <= 0)
                {
                    delayMs = 1;
                }

                // CEF asks for a single DoMessageLoopWork call after delayMs and notifies again when it
                // needs more, so this must stay one-shot: a recurring timer keeps pumping forever.
                _current = Observable.Timer(TimeSpan.FromMilliseconds(delayMs)).ObserveOn(AvaloniaScheduler.Instance).Subscribe((i) =>
                {
                    CefRuntime.DoMessageLoopWork();
                });
            }
        }
    }
}
