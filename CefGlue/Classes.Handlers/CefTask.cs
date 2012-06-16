namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface for task execution. The methods of this class may
    /// be called on any thread.
    /// </summary>
    public abstract unsafe partial class CefTask
    {
        private void execute(cef_task_t* self, CefThreadId threadId)
        {
            CheckSelf(self);

            Execute(threadId);
        }

        /// <summary>
        /// Method that will be executed.
        /// </summary>
        /// <param name="threadId">Thread executing the call.</param>
        protected abstract void Execute(CefThreadId threadId);
    }
}
