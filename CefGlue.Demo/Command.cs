namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class Command
    {
        private readonly string _text;
        private readonly EventHandler _handler;

        public Command(string text, EventHandler handler)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException("text");
            if (handler == null) throw new ArgumentNullException("handler");

            _text = text;
            _handler = handler;
        }

        public string Text { get { return _text; } }

        public void Execute()
        {
            _handler(this, EventArgs.Empty);
        }
    }
}
