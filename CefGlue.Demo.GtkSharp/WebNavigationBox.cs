namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gtk;
    using Xilium.CefGlue.Demo.Browser;

    public sealed class WebNavigationBox : Bin, IWebNavigationBox
    {
        private readonly Button _goBackButton;
        private readonly Button _goForwardButton;
        private readonly Button _stopButton;
        private readonly Button _refreshButton;
        private readonly Button _homeButton;
        private readonly Entry _addressEntry;
        private readonly Button _goButton;

        private WebBrowser _browser;

        public WebNavigationBox()
        {
            var hbox = new HBox(false, 1);

            _goBackButton = CreateButton("Back", Stock.GoBack, OnGoBack);
            hbox.Add(_goBackButton);
            ((Box.BoxChild)hbox[_goBackButton]).Expand = false;

            _goForwardButton = CreateButton("Forward", Stock.GoForward, OnGoForward);
            hbox.Add(_goForwardButton);
            ((Box.BoxChild)hbox[_goForwardButton]).Expand = false;

            _stopButton = CreateButton("Stop", Stock.Stop, OnStop);
            hbox.Add(_stopButton);
            ((Box.BoxChild)hbox[_stopButton]).Expand = false;

            _refreshButton = CreateButton("Refresh", Stock.Refresh, OnRefresh);
            hbox.Add(_refreshButton);
            ((Box.BoxChild)hbox[_refreshButton]).Expand = false;

            _homeButton = CreateButton("Home", Stock.Home, OnHome);
            hbox.Add(_homeButton);
            ((Box.BoxChild)hbox[_homeButton]).Expand = false;

            _addressEntry = new Entry();
            _addressEntry.CanFocus = true;
            hbox.Add(_addressEntry);
            ((Box.BoxChild)hbox[_addressEntry]).Expand = true;

            _goButton = CreateButton("Go", Stock.Ok, OnGo);
            hbox.Add(_goButton);
            ((Box.BoxChild)hbox[_goButton]).Expand = false;

            Add(hbox);
        }

        private Button CreateButton(string title, string stockImage, EventHandler action)
        {
            var button = new Button(new Image(stockImage, IconSize.Button));
            button.TooltipText = title;
            button.CanFocus = false;
            button.Clicked += action;
            return button;
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            if (this.Child != null)
            {
                this.Child.Allocation = allocation;
            }
        }

        protected override void OnSizeRequested(ref Requisition requisition)
        {
            if (this.Child != null)
            {
                requisition = this.Child.SizeRequest();
            }
        }

        public string HomeUrl { get; set; }

        public string Address
        {
            get { return _addressEntry.Text; }
            set { _addressEntry.Text = value; }
        }

        public bool CanGoBack
        {
            get { return _goBackButton.Sensitive; }
            set { _goBackButton.Sensitive = value; }
        }

        public bool CanGoForward
        {
            get { return _goForwardButton.Sensitive; }
            set { _goForwardButton.Sensitive = value; }
        }

        public bool Loading
        {
            get { return _stopButton.Visible; }
            set
            {
                _stopButton.Visible = value;
                _refreshButton.Visible = !value;
            }
        }

        public void NavigateTo(string url)
        {
            if (_browser != null)
            {
                _browser.CefBrowser.StopLoad();
                Address = url;
                _browser.CefBrowser.GetMainFrame().LoadUrl(url);
            }
        }

        internal void Attach(WebBrowser browser)
        {
            _browser = browser;
        }

        private void OnGoBack(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                _browser.CefBrowser.GoBack();
            }
        }

        private void OnGoForward(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                _browser.CefBrowser.GoForward();
            }
        }

        private void OnStop(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                _browser.CefBrowser.StopLoad();
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                _browser.CefBrowser.Reload();
            }
        }

        private void OnHome(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                _browser.CefBrowser.GetMainFrame().LoadUrl(HomeUrl);
            }
        }

        private void OnGo(object sender, EventArgs e)
        {
            if (_browser != null)
            {
                _browser.CefBrowser.GetMainFrame().LoadUrl(_addressEntry.Text);
            }
        }

        public CefBrowser GetBrowser()
        {
            return _browser.CefBrowser;
        }
    }
}
