namespace Xilium.CefGlue.WindowsForms
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class StatusMessageEventArgs : EventArgs
    {
        private readonly string _value;

        public StatusMessageEventArgs(string value)
        {
            _value = value;
        }

        public string Value { get { return _value; } }
    }
}
