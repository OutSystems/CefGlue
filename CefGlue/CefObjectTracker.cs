using System;
using System.Collections.Generic;
using System.Threading;

namespace Xilium.CefGlue
{
    public static class CefObjectTracker
    {
        private static bool _enabled;
        private static readonly Dictionary<int, HashSet<IDisposable>> _disposables = new Dictionary<int, HashSet<IDisposable>>();

        private class TrackingSession : IDisposable
        {
            public static readonly TrackingSession Default = new TrackingSession(stopTracking: true);
            public static readonly TrackingSession Nop = new TrackingSession(stopTracking: false);

            private readonly bool _stopTracking;

            private TrackingSession(bool stopTracking)
            {
                _stopTracking = stopTracking;
            }

            public void Dispose()
            {
                if (_stopTracking) 
                {
                    StopTracking();
                }
            }
        }

        /// <summary>
        /// Starts tracking the objects created on the current thread.
        /// </summary>
        /// <returns></returns>
        public static IDisposable StartTracking()
        {
            lock (_disposables)
            {
                _enabled = true;
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                if (!_disposables.ContainsKey(currentThreadId))
                {
                    // no sessions have started in current thread
                    _disposables.Add(currentThreadId, new HashSet<IDisposable>());
                    return TrackingSession.Default;
                }
                return TrackingSession.Nop; // return a nop session, won't stop tracking, the outer most session should do the job
            }
        }

        /// <summary>
        /// Disposes the objects registered on the current thread since tracker was started.
        /// </summary>
        public static void StopTracking()
        {
            lock (_disposables)
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                if (_disposables.TryGetValue(currentThreadId, out var disposables))
                {
                    _disposables.Remove(currentThreadId);
                    foreach (var disposable in disposables)
                    {
                        disposable.Dispose();
                    }
                }
                _enabled = _disposables.Count > 0;
            }
        }

        /// <summary>
        /// Registers the specified object to be disposed later.
        /// </summary>
        /// <param name="obj"></param>
        public static void Track(IDisposable obj)
        {
            if (_enabled)
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                lock (_disposables)
                {
                    if (_disposables.TryGetValue(currentThreadId, out var threadDisposables))
                    {
                        threadDisposables.Add(obj);
                    }
                }
            }
        }
        
        /// <summary>
        /// Removes the specified object from the tracking list.
        /// </summary>
        /// <param name="obj"></param>
        public static void Untrack(IDisposable obj)
        {
            if (_enabled)
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                lock (_disposables)
                {
                    if (_disposables.TryGetValue(currentThreadId, out var threadDisposables))
                    {
                        threadDisposables.Remove(obj);
                    }
                }
            }
        }
    }
}
