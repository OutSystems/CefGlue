using System;

namespace Xilium.CefGlue.Common.Shared.Helpers
{
    internal sealed class ActionTask : CefTask
    {
        private Action _action;

        public ActionTask(Action action)
        {
            _action = action;
        }

        protected override void Execute()
        {
            _action();
            _action = null;
        }

        public static void Run(Action action, CefThreadId threadId = CefThreadId.UI)
        {
            CefRuntime.PostTask(threadId, new ActionTask(action));
        }
    }
}
