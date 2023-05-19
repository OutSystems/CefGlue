using System;
using System.Collections.Generic;
using System.Linq;

namespace Xilium.CefGlue
{
    public static class CefObjectTracker
    {
        [ThreadStatic]
        private static HashSet<IDisposable> _disposables;

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
            if (_disposables != null)
            {
                return TrackingSession.Nop; // return a nop session, won't stop tracking, the outer most session should do the job
            }
            _disposables = new HashSet<IDisposable>();
            return TrackingSession.Default;
        }

        /// <summary>
        /// Disposes the objects registered on the current thread since tracker was started.
        /// </summary>
        public static void StopTracking()
        {
            if (_disposables == null)
            {
                return;
            }

            var disposables = _disposables.ToArray();
            _disposables = null; // set null before running dispose to prevent objects running untrack

            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Registers the specified object to be disposed later.
        /// </summary>
        /// <param name="obj"></param>
        public static void Track(IDisposable obj)
        {
            if (_disposables != null)
            {
                _disposables.Add(obj);
            }
        }

        /// <summary>
        /// Removes the specified object from the tracking list.
        /// </summary>
        /// <param name="obj"></param>
        public static void Untrack(IDisposable obj)
        {
            if (_disposables != null)
            {
                _disposables.Remove(obj);
            }
        }
    }
}
