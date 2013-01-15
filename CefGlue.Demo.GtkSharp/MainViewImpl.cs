namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Gtk;
    using MenuItemImpl = Gtk.MenuItem;

    internal sealed class MainViewImpl : Window, IMainView
    {
        private readonly DemoApp _application;
        private readonly string _applicationTitle;

        private readonly Notebook _tabs;

        public MainViewImpl(DemoApp application, MenuItem[] menuItems)
            : base(WindowType.Toplevel)
        {
            _application = application;
            var menuBar = CreateMenu(menuItems);

            _applicationTitle = _application.Name + " (GtkSharp)";
            Title = _applicationTitle;
            Resize(_application.DefaultWidth, _application.DefaultHeight);

            Destroyed += MainViewImpl_Destroyed;

            var vbox = new VBox(false, 0);
            Add(vbox);

            vbox.PackStart(menuBar, false, false, 2);

            _tabs = new Notebook();
            vbox.PackEnd(_tabs, true, true, 2);

            ShowAll();
        }

        public void Close()
        {
            Console.WriteLine("Close()");
            Destroy();
        }

        private void MainViewImpl_Destroyed(object sender, EventArgs e)
        {
            Console.WriteLine("MainViewImpl_Destroyed()");
            _application.Quit();
        }

        #region CreateMenu

        private MenuBar CreateMenu(MenuItem[] items)
        {
            var acc = new MenuBar();
            foreach (var item in items)
                acc.Append(Map(item));
            return acc;
        }

        private Menu Map(IEnumerable<MenuItem> items)
        {
            var acc = new Menu();
            foreach (var item in items) acc.Append(Map(item));
            return acc;
        }

        private MenuItemImpl Map(MenuItem item)
        {
            var result = new MenuItemImpl(item.Text);

            if (item.Items != null)
            {
                result.Submenu = Map(item.Items);
            }
            else if (item.Command != null)
            {
                result.Activated += (s, e) => { item.Command.Execute(); };
            }

            return result;
        }

        #endregion

        public void NewTab(string url)
        {
            var state = new WebBrowserState();

            var label = new Label(url);
            var content = new VBox(true, 0);
            content.Homogeneous = false;

            var navBox = new WebNavigationBox();
            content.Add(navBox);
            ((Box.BoxChild)content[navBox]).Expand = false;
            ((Box.BoxChild)content[navBox]).Fill = false;
            navBox.HomeUrl = _application.HomeUrl;

            var browserCtl = new CefWebBrowser();
            content.Add(browserCtl);
            ((Box.BoxChild)content[browserCtl]).Expand = true;
            ((Box.BoxChild)content[browserCtl]).Fill = true;

            var browser = browserCtl.WebBrowser;
            browser.StartUrl = url;

            navBox.Attach(browser);

            browser.TitleChanged += (s, e) =>
            {
                state.Title = e.Title;
                Application.Invoke((_s, _e) => { UpdateTitle(e.Title); });
            };

            browser.AddressChanged += (s, e) =>
            {
                state.Title = e.Address;
                Application.Invoke((_s, _e) => { navBox.Address = e.Address; });
            };

            browser.TargetUrlChanged += (s, e) =>
            {
                state.TargetUrl = e.TargetUrl;
                // TODO: show targeturl in status bar
                // Application.Invoke((_s, _e) => { UpdateTargetUrl(e.TargetUrl); });
            };

            browser.LoadingStateChanged += (s, e) =>
            {
                Application.Invoke((_s, _e) =>
                {
                    navBox.CanGoBack = e.CanGoBack;
                    navBox.CanGoForward = e.CanGoForward;
                    navBox.Loading = e.Loading;
                });
            };

            _tabs.AppendPage(content, label);
            content.ShowAll();
            _tabs.CurrentPage = _tabs.PageNum(content);
        }

        private void UpdateTitle(string title)
        {
            Title = string.IsNullOrEmpty(title) ? _applicationTitle : title + " - " + _applicationTitle;
        }

        private WebNavigationBox GetCurrentNavBox()
        {
            var widget = _tabs.CurrentPageWidget;
            var container = (widget as Gtk.Container);
            foreach (var w in container.Children)
            {
                if (w is WebNavigationBox)
                {
                    return (WebNavigationBox)w;
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
            throw new NotImplementedException();
        }
    }
}
