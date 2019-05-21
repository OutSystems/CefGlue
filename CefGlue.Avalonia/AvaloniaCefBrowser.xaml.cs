using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Avalonia
{
    public class AvaloniaCefBrowser : TemplatedControl, IDisposable
    {

        private readonly ILogger _logger;
        private readonly AvaloniaBrowserAdapter _adapter;

        public AvaloniaCefBrowser() : this(new NLogLogger(nameof(AvaloniaCefBrowser)))
        {
        }

        private AvaloniaCefBrowser(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
            _adapter = new AvaloniaBrowserAdapter(this, logger);
        }

        #region Disposable

        ~AvaloniaCefBrowser()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _adapter.Dispose(disposing);
        }

        #endregion

        public event LoadStartEventHandler LoadStart { add => _adapter.LoadStart += value ; remove => _adapter.LoadStart -= value; }
        public event LoadEndEventHandler LoadEnd { add => _adapter.LoadEnd += value; remove => _adapter.LoadEnd -= value; }
        public event LoadingStateChangeEventHandler LoadingStateChange { add => _adapter.LoadingStateChange += value; remove => _adapter.LoadingStateChange -= value; }
        public event LoadErrorEventHandler LoadError { add => _adapter.LoadError += value; remove => _adapter.LoadError -= value; }

        public string StartUrl { get => _adapter.StartUrl; set => _adapter.StartUrl = value; }

        public bool AllowsTransparency { get => _adapter.AllowsTransparency; set => _adapter.AllowsTransparency = value; }

        public void NavigateTo(string url)
        {
            _adapter.NavigateTo(url);
        }

        public void LoadString(string content, string url)
        {
            _adapter.LoadString(content, url);
        }

        public bool CanGoBack()
        {
            return _adapter.CanGoBack();
        }

        public void GoBack()
        {
            _adapter.GoBack();
        }

        public bool CanGoForward()
        {
            return _adapter.CanGoForward();
        }

        public void GoForward()
        {
            _adapter.GoForward();
        }

        public void Refresh()
        {
            _adapter.Refresh();
        }

        public void ExecuteJavaScript(string code, string url, int line)
        {
            _adapter.ExecuteJavaScript(code, url, line);
        }

        public void ShowDeveloperTools()
        {
            _adapter.ShowDeveloperTools();
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            _adapter.BrowserImage = e.NameScope.Find<Image>("PART_Image");
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);

            if (_adapter.BrowserImage != null)
            {
                _adapter.CreateOrUpdateBrowser((int) size.Width, (int) size.Height);
            }

            return size;
        }
    }
}
