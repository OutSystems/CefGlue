using System;
using System.Collections.Generic;
using System.Threading;

namespace Xilium.CefGlue
{
    public static class CefObjectTracker
    {
        private static bool _enabled;
        private static readonly Dictionary<int, List<IDisposable>> _disposables = new Dictionary<int, List<IDisposable>>();

        private class TrackSession : IDisposable
        {
            public static readonly TrackSession Default = new TrackSession();

            private TrackSession() { }

            public void Dispose()
            {
                StopTracking();
            }
        }

        public static IDisposable StartTracking()
        {
            lock (_disposables)
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                if (!_disposables.ContainsKey(currentThreadId))
                {
                    _disposables.Add(currentThreadId, new List<IDisposable>());
                }
                _enabled = true;
            }
            return TrackSession.Default;
        }

        public static void StopTracking()
        {
            System.Diagnostics.Debugger.Launch();
            lock (_disposables)
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                if (_disposables.TryGetValue(currentThreadId, out var disposables))
                {
                    _disposables.Remove(Thread.CurrentThread.ManagedThreadId);
                    foreach (var disposable in disposables)
                    {
                        disposable.Dispose();
                    }
                }
                _enabled = _disposables.Count > 0;
            }
        }

        public static void Track(IDisposable obj) 
        {
            if (_enabled) {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                if (_disposables.TryGetValue(currentThreadId, out var threadDisposables))
                {
                    threadDisposables.Add(obj);
                }
            }
        }
        
        public static void Untrack(IDisposable obj, bool dispose = true)
        {
            if (_enabled)
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                if (_disposables.TryGetValue(currentThreadId, out var threadDisposables))
                {
                    threadDisposables.Remove(obj);
                }
            }
        }
    }
}
