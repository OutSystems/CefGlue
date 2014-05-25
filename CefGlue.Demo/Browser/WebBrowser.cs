namespace Xilium.CefGlue.Demo.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public sealed class WebBrowser
    {
        private readonly object _owner;
        private readonly CefBrowserSettings _settings;
        private string _startUrl;
        private CefClient _client;
        private CefBrowser _browser;
        private IntPtr _windowHandle;

        private bool _created;

        public WebBrowser(object owner, CefBrowserSettings settings, string startUrl)
        {
            _owner = owner;
            _settings = settings;
            _startUrl = startUrl;
        }

        public string StartUrl
        {
            get { return _startUrl; }
            set { _startUrl = value; }
        }

        public CefBrowser CefBrowser
        {
            get { return _browser; }
        }

        public void Create(CefWindowInfo windowInfo)
        {
            if (_client == null)
            {
                _client = new WebClient(this);
            }

            CefBrowserHost.CreateBrowser(windowInfo, _client, _settings, StartUrl);
        }

        public event EventHandler Created;

        internal void OnCreated(CefBrowser browser)
        {
            //if (_created) throw new InvalidOperationException("Browser already created.");
            _created = true;
            _browser = browser;

            var handler = Created;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public void Close()
        {
            if (_browser != null)
            {
                var host = _browser.GetHost();
                host.CloseBrowser(true);
                host.Dispose();
                _browser.Dispose();
                _browser = null;
            }
        }

        public event EventHandler<TitleChangedEventArgs> TitleChanged;

        internal void OnTitleChanged(string title)
        {
            var handler = TitleChanged;
            if (handler != null)
            {
                handler(this, new TitleChangedEventArgs(title));
            }
        }

        public event EventHandler<AddressChangedEventArgs> AddressChanged;

        internal void OnAddressChanged(string address)
        {
            var handler = AddressChanged;
            if (handler != null)
            {
                handler(this, new AddressChangedEventArgs(address));
            }
        }

        public event EventHandler<TargetUrlChangedEventArgs> TargetUrlChanged;

        internal void OnTargetUrlChanged(string targetUrl)
        {
            var handler = TargetUrlChanged;
            if (handler != null)
            {
                handler(this, new TargetUrlChangedEventArgs(targetUrl));
            }
        }

        public event EventHandler<LoadingStateChangedEventArgs> LoadingStateChanged;

        internal void OnLoadingStateChanged(bool isLoading, bool canGoBack, bool canGoForward)
        {
            var handler = LoadingStateChanged;
            if (handler != null)
            {
                handler(this, new LoadingStateChangedEventArgs(isLoading, canGoBack, canGoForward));
            }
        }
    }
}
