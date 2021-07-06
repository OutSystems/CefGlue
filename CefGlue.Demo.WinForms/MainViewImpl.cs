namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;
    using MenuItemImpl = System.Windows.Forms.ToolStripMenuItem;

    internal sealed class MainViewImpl : Form, IMainView
    {
        private readonly DemoApp _application;
        private readonly string _applicationTitle;
        private readonly TabControl _tabs;

        private readonly SynchronizationContext _pUIThread;

        public MainViewImpl(DemoApp application, MenuItem[] menuItems)
        {
            _pUIThread = WindowsFormsSynchronizationContext.Current;

            _application = application;
            CreateMenu(menuItems);

            _applicationTitle = _application.Name + " (Windows Forms)";
            Text = _applicationTitle;
            Size = new Size(_application.DefaultWidth, _application.DefaultHeight);

            Padding = new Padding(0, 0, 0, 0);

            _tabs = new TabControl();
            _tabs.Parent = this;
            _tabs.Margin = new Padding(10, 10, 10, 10);
            _tabs.Dock = DockStyle.Fill;

            _tabs.Appearance = TabAppearance.Normal;
            _tabs.Padding = new Point(6, 6);

            if (MainMenuStrip != null)
            {
                Controls.Add(MainMenuStrip);
            }

            Visible = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            foreach (TabPage page in _tabs.TabPages)
            {
                var browser = page.Tag as CefWebBrowser;
                if (browser != null)
                {
                    browser.Dispose();
                }
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _application.Quit();
        }

        #region CreateMenu

        private void CreateMenu(MenuItem[] items)
        {
            var menu = new MenuStrip();
            menu.Items.AddRange(Map(items));
            menu.Dock = DockStyle.Top;
            MainMenuStrip = menu;
        }

        private MenuItemImpl[] Map(IEnumerable<MenuItem> items)
        {
            var acc = new List<MenuItemImpl>();
            foreach (var item in items) acc.Add(Map(item));
            return acc.ToArray();
        }

        private MenuItemImpl Map(MenuItem item)
        {
            if (item.Items != null)
                return new MenuItemImpl(item.Text, image: null, Map(item.Items));
            else if (item.Command != null)
                return new MenuItemImpl(item.Text, image: null, (s, e) => { item.Command.Execute(); });
            else return new MenuItemImpl(item.Text);
        }

        #endregion

        public void NewTab(string url)
        {
            var state = new WebBrowserState();

            var tabPage = new TabPage(url);

            var navBox = new WebNavigationBox();
            navBox.Parent = tabPage;
            navBox.Dock = DockStyle.Top;
            navBox.Visible = true;
            navBox.HomeUrl = _application.HomeUrl;

            var browserCtl = new CefWebBrowser();
            tabPage.Tag = browserCtl;
            browserCtl.Parent = tabPage;
            browserCtl.Dock = DockStyle.Fill;
            browserCtl.BringToFront();

            var browser = browserCtl.WebBrowser;
            browser.StartUrl = url;

            navBox.Attach(browser);

            browser.TitleChanged += (s, e) =>
                {
                    state.Title = e.Title;
                    _pUIThread.Post((_state) => { UpdateTitle(e.Title); }, null);
                };

            browser.AddressChanged += (s, e) =>
                {
                    state.Title = e.Address;
                    _pUIThread.Post((_state) => { navBox.Address = e.Address; }, null);
                };

            browser.TargetUrlChanged += (s, e) =>
                {
                    state.TargetUrl = e.TargetUrl;
                    // TODO: show targeturl in status bar
                    // _pUIThread.Post((_state) => { UpdateTargetUrl(e.TargetUrl); }, null);
                };

            browser.LoadingStateChanged += (s, e) =>
                {
                    _pUIThread.Post((_state) =>
                        {
                            navBox.CanGoBack = e.CanGoBack;
                            navBox.CanGoForward = e.CanGoForward;
                            navBox.Loading = e.Loading;
                        }, null);
                };

            _tabs.TabPages.Add(tabPage);
            _tabs.SelectedTab = tabPage;
        }

        private void UpdateTitle(string title)
        {
            Text = string.IsNullOrEmpty(title) ? _applicationTitle : title + " - " + _applicationTitle;
        }

        private WebNavigationBox GetCurrentNavBox()
        {
            var tab = _tabs.SelectedTab;
            if (tab == null) return null;
            foreach (var c in tab.Controls)
            {
                if (c is WebNavigationBox)
                {
                    return (WebNavigationBox)c;
                }
            }
            return null;
        }

        public void NavigateTo(string url)
        {
            var navBox = GetCurrentNavBox();
            navBox.NavigateTo(url);
        }

        public CefBrowser CurrentBrowser
        {
            get
            {
                var navBox = GetCurrentNavBox();
                if (navBox == null) return null;
                return navBox.GetBrowser();
            }
        }

        public void NewWebView(string url, bool transparent)
        {
            var view = new CefWebView(url, transparent);
        }
    }
}
