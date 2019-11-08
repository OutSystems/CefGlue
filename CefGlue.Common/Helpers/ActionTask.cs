using System;

namespace Xilium.CefGlue.Common.Helpers
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
    }
}
