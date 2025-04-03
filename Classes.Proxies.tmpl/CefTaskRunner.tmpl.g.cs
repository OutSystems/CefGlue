namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that asynchronously executes tasks on the associated thread. It is
    /// safe to call the methods of this class on any thread.
    /// CEF maintains multiple internal threads that are used for handling different
    /// types of tasks in different processes. The cef_thread_id_t definitions in
    /// cef_types.h list the common CEF threads. Task runners are also available for
    /// other CEF threads as appropriate (for example, V8 WebWorker threads).
    /// </summary>
    public sealed unsafe partial class CefTaskRunner
    {
        /// <summary>
        /// Returns the task runner for the current thread. Only CEF threads will have
        /// task runners. An empty reference will be returned if this method is called
        /// on an invalid thread.
        /// </summary>
        public static cef_task_runner_t* GetForCurrentThread()
        {
            throw new NotImplementedException(); // TODO: CefTaskRunner.GetForCurrentThread
        }
        
        /// <summary>
        /// Returns the task runner for the specified CEF thread.
        /// </summary>
        public static cef_task_runner_t* GetForThread(CefThreadId threadId)
        {
            throw new NotImplementedException(); // TODO: CefTaskRunner.GetForThread
        }
        
        /// <summary>
        /// Returns true if this object is pointing to the same task runner as |that|
        /// object.
        /// </summary>
        public int IsSame(cef_task_runner_t* that)
        {
            throw new NotImplementedException(); // TODO: CefTaskRunner.IsSame
        }
        
        /// <summary>
        /// Returns true if this task runner belongs to the current thread.
        /// </summary>
        public int BelongsToCurrentThread()
        {
            throw new NotImplementedException(); // TODO: CefTaskRunner.BelongsToCurrentThread
        }
        
        /// <summary>
        /// Returns true if this task runner is for the specified CEF thread.
        /// </summary>
        public int BelongsToThread(CefThreadId threadId)
        {
            throw new NotImplementedException(); // TODO: CefTaskRunner.BelongsToThread
        }
        
        /// <summary>
        /// Post a task for execution on the thread associated with this task runner.
        /// Execution will occur asynchronously.
        /// </summary>
        public int PostTask(cef_task_t* task)
        {
            throw new NotImplementedException(); // TODO: CefTaskRunner.PostTask
        }
        
        /// <summary>
        /// Post a task for delayed execution on the thread associated with this task
        /// runner. Execution will occur asynchronously. Delayed tasks are not
        /// supported on V8 WebWorker threads and will be executed without the
        /// specified delay.
        /// </summary>
        public int PostDelayedTask(cef_task_t* task, long delay_ms)
        {
            throw new NotImplementedException(); // TODO: CefTaskRunner.PostDelayedTask
        }
        
    }
}
