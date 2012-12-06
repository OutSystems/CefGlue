namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using WebBrowser = Xilium.CefGlue.Demo.Browser.WebBrowser;

    public partial class WebNavigationBox : Control
    {
        private readonly Button _goBackButton;
        private readonly Button _goForwardButton;
        private readonly Button _stopButton;
        private readonly Button _refreshButton;
        private readonly Button _homeButton;
        private readonly TextBox _addressTextBox;
        private readonly Button _goButton;

        private WebBrowser _browser;

        public WebNavigationBox()
        {
            InitializeComponent();

            Height = 36;

            _goBackButton = CreateButton("Back", "appbar.arrow.left.png", OnGoBack);
            _goForwardButton = CreateButton("Forward", "appbar.arrow.right.png", OnGoForward);
            _stopButton = CreateButton("Stop", "appbar.close.png", OnStop);
            _refreshButton = CreateButton("Refresh", "appbar.refresh.png", OnRefresh);
            _homeButton = CreateButton("Home", "appbar.home.png", OnHome);
            _goButton = CreateButton("Go", "go.png", OnGo, DockStyle.Right);

            _addressTextBox = new TextBox();
            _addressTextBox.Parent = this;
            _addressTextBox.Dock = DockStyle.Fill;
            _addressTextBox.BringToFront();

            _addressTextBox.KeyPress += (s, e) =>
                {
                    if (e.KeyChar == '\xD') { e.Handled = true; OnGo(s, EventArgs.Empty); }
                };

            CanGoBack = false;
            CanGoForward = false;
            Loading = false;
        }

        // TODO: fix button creation using ImageList, and resize images to other size, tooltips.
        // Yes, we doesn't use any *Strip controls, 'cause from one side they are really buggy, and from other side - this code more similar to GTK port.
        private Button CreateButton(string title, string stockImage, EventHandler action, DockStyle dockStyle = DockStyle.Left)
        {
            var button = new Button();
            button.Image = Image.FromStream(GetImageStream(stockImage));
            button.Width = 36; // button.Image.Width;
            button.Height = 36; // button.Image.Height;
            button.Text = "";
            button.TextImageRelation = TextImageRelation.Overlay;
            button.ImageAlign = ContentAlignment.MiddleCenter;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.Parent = this;
            button.Dock = dockStyle;
            button.BringToFront();
            button.Click += action;
            return button;
        }

        private static Stream GetImageStream(string name)
        {
            var type = typeof(WebNavigationBox);
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".Resources." + name);
        }

        public string HomeUrl { get; set; }

        public string Address
        {
            get { return _addressTextBox.Text; }
            set { _addressTextBox.Text = value; }
        }

        public bool CanGoBack
        {
            get { return _goBackButton.Enabled; }
            set { _goBackButton.Enabled = value; }
        }

        public bool CanGoForward
        {
            get { return _goForwardButton.Enabled; }
            set { _goForwardButton.Enabled = value; }
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

        public void Attach(WebBrowser browser)
        {
            _browser = browser;
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
                _browser.CefBrowser.GetMainFrame().LoadUrl(_addressTextBox.Text);
            }
        }

        public CefBrowser GetBrowser()
        {
            return _browser.CefBrowser;
        }
    }
}
