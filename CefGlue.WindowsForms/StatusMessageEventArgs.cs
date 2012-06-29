namespace Xilium.CefGlue.WindowsForms
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class StatusMessageEventArgs : EventArgs
    {
        private readonly string _value;
        private readonly CefStatusMessageType _type;

        public StatusMessageEventArgs(string value, CefStatusMessageType type)
        {
            _value = value;
            _type = type;
        }

        public string Value { get { return _value; } }

        public CefStatusMessageType MessageType { get { return _type; } }
    }
}
