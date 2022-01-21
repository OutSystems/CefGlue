using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal static class GenericTaskAwaiter
    {
        private static readonly ConcurrentDictionary<Type, Func<Task, object>> _taskResultMethods = new ConcurrentDictionary<Type, Func<Task, object>>();
        private static readonly Func<Task, object> _noop = t => null;

        static GenericTaskAwaiter()
        {
            _taskResultMethods.TryAdd(Task.CompletedTask.GetType(), _noop);
        }

        public static (object Result, Exception Exception) GetResultFrom(Task task)
        {
            if (task.IsFaulted)
            {
                return (default, task.Exception);
            }
           
            try
            {
                // get task method to obtain result and cache it to improve performance of recurrent calls 
                var resultGetter = _taskResultMethods.GetOrAdd(task.GetType(), (t) => GetResultGetter(t));
                return (resultGetter(task), default);
            }
            catch (Exception e)
            {
                return (default, e);
            }
        }

        /// <summary>
        /// Create an helper function to obtain the result from a task.
        /// </summary>
        private static Func<Task, object> GetResultGetter(Type taskType)
        {
            if (typeof(Task).IsAssignableFrom(taskType))
            {
                if (taskType.IsGenericType)
                {
                    // generic Task<T>
                    var resultGetter = taskType.GetProperty(nameof(Task<object>.Result));
                    return (task) => resultGetter.GetValue(task);
                }

                // non-generic Task
                return _noop;
            }

            throw new ArgumentException("Expected a Task type", nameof(taskType));
        }
    }
}
