namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that facilitates managing the browser-related tasks.
    /// The methods of this class may only be called on the UI thread.
    /// </summary>
    public sealed unsafe partial class CefTaskManager
    {
        /// <summary>
        /// Returns the global task manager object.
        /// Returns nullptr if the method was called from the incorrect thread.
        /// </summary>
        public static cef_task_manager_t* GetTaskManager()
        {
            throw new NotImplementedException(); // TODO: CefTaskManager.GetTaskManager
        }
        
        /// <summary>
        /// Returns the number of tasks currently tracked by the task manager.
        /// Returns 0 if the method was called from the incorrect thread.
        /// </summary>
        public UIntPtr GetTasksCount()
        {
            throw new NotImplementedException(); // TODO: CefTaskManager.GetTasksCount
        }
        
        /// <summary>
        /// Gets the list of task IDs currently tracked by the task manager. Tasks
        /// that share the same process id will always be consecutive. The list will
        /// be sorted in a way that reflects the process tree: the browser process
        /// will be first, followed by the gpu process if it exists. Related processes
        /// (e.g., a subframe process and its parent) will be kept together if
        /// possible. Callers can expect this ordering to be stable when a process is
        /// added or removed. The task IDs are unique within the application lifespan.
        /// Returns false if the method was called from the incorrect thread.
        /// </summary>
        public int GetTaskIdsList(UIntPtr* task_idsCount, long* task_ids)
        {
            throw new NotImplementedException(); // TODO: CefTaskManager.GetTaskIdsList
        }
        
        /// <summary>
        /// Gets information about the task with |task_id|.
        /// Returns true if the information about the task was successfully
        /// retrieved and false if the |task_id| is invalid or the method was called
        /// from the incorrect thread.
        /// </summary>
        public int GetTaskInfo(long task_id, cef_task_info_t* info)
        {
            throw new NotImplementedException(); // TODO: CefTaskManager.GetTaskInfo
        }
        
        /// <summary>
        /// Attempts to terminate a task with |task_id|.
        /// Returns false if the |task_id| is invalid, the call is made from an
        /// incorrect thread, or if the task cannot be terminated.
        /// </summary>
        public int KillTask(long task_id)
        {
            throw new NotImplementedException(); // TODO: CefTaskManager.KillTask
        }
        
        /// <summary>
        /// Returns the task ID associated with the main task for |browser_id|
        /// (value from CefBrowser::GetIdentifier). Returns -1 if |browser_id| is
        /// invalid, does not currently have an associated task, or the method was
        /// called from the incorrect thread.
        /// </summary>
        public long GetTaskIdForBrowserId(int browser_id)
        {
            throw new NotImplementedException(); // TODO: CefTaskManager.GetTaskIdForBrowserId
        }
        
    }
}
