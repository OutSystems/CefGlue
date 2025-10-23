using System;
using System.Reactive.Linq;

namespace Xilium.CefGlue
{
    internal class BrowserProcessHandler : Common.Handlers.BrowserProcessHandler
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

                _current = Observable.Interval(TimeSpan.FromMilliseconds(delayMs)).Subscribe((i) =>
                {
                    CefRuntime.DoMessageLoopWork();
                });
            }
        }
    }
}
