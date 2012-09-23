namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class MenuItem
    {
        private readonly string _text;
        private readonly Command _command;
        private readonly MenuItem[] _items;

        public MenuItem(string text)
            : this(text, null, null)
        { }

        public MenuItem(string text, MenuItem[] items)
            : this(text, null, items)
        { }

        public MenuItem(Command command)
            : this(null, command, null)
        { }

        public MenuItem(Command command, MenuItem[] items)
            : this(null, command, null)
        { }

        private MenuItem(string text, Command command, MenuItem[] items)
        {
            _text = text ?? command.Text;
            _command = command;
            _items = items;
        }

        public string Text { get { return _text ?? _command.Text; } }
        public Command Command { get { return _command; } }
        public IEnumerable<MenuItem> Items { get { return _items; } }
    }
}
