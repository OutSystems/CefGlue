namespace Xilium.CefGlue.Demo.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class AddressChangedEventArgs : EventArgs
    {
        private readonly string _address;

        public AddressChangedEventArgs(string address)
        {
            _address = address;
        }

        public string Address
        {
            get { return _address; }
        }
    }
}
