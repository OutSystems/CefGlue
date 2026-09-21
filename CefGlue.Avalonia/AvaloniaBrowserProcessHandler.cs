using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Xilium.CefGlue.Common.Handlers;

namespace Xilium.CefGlue.Avalonia
{
    internal class AvaloniaBrowserProcessHandler : BrowserProcessHandler
    {
        // CEF only forecasts work it already knows about, and stops notifying once the
        // pump stalls, so DoMessageLoopWork has to be polled on a free-running timer.
        // 30 Hz is what cefclient's MainMessageLoopExternalPump (kMaxTimerDelay) and
        // CefSharp's WpfBrowserProcessHandler both settle on. The delay CEF requests is
        // deliberately never used as the timer period: it is a deadline, not an
        // interval, and treating it as one turned the usual request of 0 into a 1 kHz
        // pump that burned a CPU core while idle.
        private const int DefaultPumpIntervalMs = 1000 / 30;

        // Escape hatch for diagnostics and for hosts that want 60 Hz. Clamped, because
        // a very short interval reintroduces the CPU burn and a very long one starves
        // Chromium of the time slices it needs to notice new work.
        private const int MinPumpIntervalMs = 1000 / 60;
        private const int MaxPumpIntervalMs = 1000;
        private const string PumpIntervalSetting = "CefGlue.Avalonia.MessagePumpIntervalMs";
        private const string PumpIntervalVariable = "CEFGLUE_MESSAGE_PUMP_INTERVAL_MS";

        private static readonly int PumpIntervalMs = ResolvePumpIntervalMs();

        private IDisposable _pump;
        private object _schedule = new object();
        private int _immediatePumpQueued;

        private static int ResolvePumpIntervalMs()
        {
            var data = AppContext.GetData(PumpIntervalSetting);
            var configured = data != null
                ? Convert.ToString(data, CultureInfo.InvariantCulture)
                : Environment.GetEnvironmentVariable(PumpIntervalVariable);

            if (!int.TryParse(configured, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intervalMs))
            {
                return DefaultPumpIntervalMs;
            }

            return Math.Clamp(intervalMs, MinPumpIntervalMs, MaxPumpIntervalMs);
        }

        protected override void OnScheduleMessagePumpWork(long delayMs)
        {
            lock (_schedule)
            {
                if (_pump == null)
                {
                    _pump = Observable.Interval(TimeSpan.FromMilliseconds(PumpIntervalMs)).ObserveOn(AvaloniaScheduler.Instance).Subscribe((i) =>
                    {
                        CefRuntime.DoMessageLoopWork();
                    });
                }
            }

            if (delayMs > 0)
            {
                return;
            }

            // CEF wants work as soon as possible, so don't make it wait for the next tick. This
            // notification arrives from any thread and asks for 0 the vast majority of the time,
            // so only keep one immediate pump in flight: a single DoMessageLoopWork drains
            // everything that is pending.
            if (Interlocked.Exchange(ref _immediatePumpQueued, 1) == 1)
            {
                return;
            }

            // Default, not Normal: in Avalonia, Default is the normal priority (0) and Normal is
            // a WPF-compatibility alias sitting second from the top (5), above Render (4), so
            // pumping at Normal lets CEF preempt layout and repaint. Not Background or Input
            // either: Dispatcher.ExecuteJobsCore runs everything at Input priority and below only
            // while the platform reports no pending OS input, which would defer the pump exactly
            // while the user is typing or scrolling. Default is the lowest priority that is still
            // dispatched unconditionally.
            Dispatcher.UIThread.Post(
                () =>
                {
                    // Cleared before the pump runs, so work CEF schedules from inside
                    // DoMessageLoopWork still queues a fresh notification.
                    Volatile.Write(ref _immediatePumpQueued, 0);
                    CefRuntime.DoMessageLoopWork();
                },
                DispatcherPriority.Default);
        }
    }
}
