namespace Xilium.CefGlue.Client
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Xilium.CefGlue.WindowsForms;

    public partial class MainForm : Form
    {
        private readonly string _mainTitle;

        public MainForm()
        {
            InitializeComponent();

            _mainTitle = Text;

            NewTab("http://google.com");
        }

        private CefWebBrowser GetActiveBrowser()
        {
            if (tabControl.TabCount > 0)
            {
                var page = tabControl.TabPages[tabControl.SelectedIndex];
                foreach (var ctl in page.Controls)
                {
                    if (ctl is CefWebBrowser)
                    {
                        var browser = (CefWebBrowser)ctl;
                        return browser;
                    }
                }
            }

            return null;
        }

        void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.TabCount > 0)
            {
                var page = tabControl.TabPages[tabControl.SelectedIndex];
                foreach (var ctl in page.Controls)
                {
                    if (ctl is CefWebBrowser)
                    {
                        var browser = (CefWebBrowser)ctl;

                        Text = browser.Title + " - " + _mainTitle;

                        break;
                    }
                }
            }
            else
            {
                Text = _mainTitle;
            }
        }

        private void tabControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabControl.TabCount; i++)
                {
                    Rectangle r = tabControl.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        closeTabContextMenuItem.Tag = tabControl.TabPages[i];
                        tabContextMenu.LostFocus += (s, ev) => { tabContextMenu.Hide(); };
                        tabContextMenu.ChangeUICues += (s, ev) => { tabContextMenu.Hide(); };
                        tabContextMenu.Show(tabControl, e.Location);
                    }
                }
            }
        }

        private void newTabAction(object sender, EventArgs e)
        {
            NewTab("http://google.com");
        }

        private void goAddressAction(object sender, EventArgs e)
        {
            var ctl = GetActiveBrowser();
            if (ctl != null)
            {
                ctl.Browser.GetMainFrame().LoadUrl(addressTextBox.Text);
            }
        }

        private void NewTab(string startUrl)
        {
            var page = new TabPage("New Tab");
            page.Padding = new Padding(0, 0, 0, 0);

            var browser = new CefWebBrowser();
            browser.StartUrl = startUrl;
            browser.Dock = DockStyle.Fill;
            browser.TitleChanged += (s, e) =>
                {
                    BeginInvoke(new Action(() => {
                        var title = browser.Title;
                        if (tabControl.SelectedTab == page)
                        {
                            title = Text = browser.Title + " - " + _mainTitle;
                        }
                        page.ToolTipText = title;
                        if (title.Length > 18)
                        {
                            title = title.Substring(0, 18) + "...";
                        }
                        page.Text = title;
                    }));
                };
            browser.AddressChanged += (s, e) =>
                {
                    BeginInvoke(new Action(() => {
                        addressTextBox.Text = browser.Address;
                    }));
                };
            browser.StatusMessage += (s, e) =>
                {
                    BeginInvoke(new Action(() => {
                        statusLabel.Text = e.Value;
                    }));
                };

            page.Controls.Add(browser);

            tabControl.TabPages.Add(page);

            tabControl.SelectedTab = page;
        }

        private void closeTabAction(object sender, EventArgs e)
        {
            var s = (ToolStripMenuItem)sender;
            var page = s.Tag as TabPage;
            if (page != null)
            {
                page.Dispose();
                page = null;
            }
        }

        private void addressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) goAddressAction(sender, EventArgs.Empty);
        }
    }
}
