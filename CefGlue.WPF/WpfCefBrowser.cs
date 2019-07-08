using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.WPF.Platform;

namespace Xilium.CefGlue.WPF
{
    public class WpfCefBrowser : ContentControl, IDisposable
    {
        private readonly ILogger _logger;
        private readonly CommonBrowserAdapter _adapter;
        private readonly IDisposable[] _disposables;

        public WpfCefBrowser() {
            _logger = new Logger(nameof(WpfCefBrowser));

            var image = CreateImage();
            Content = image;

            var popupImage = CreateImage();
            var popup = new Popup
            {
                PlacementTarget = this,
                Placement = PlacementMode.Relative,
                Child = popupImage
            };

            var renderHandler = new WpfRenderHandler(image, _logger);
            var controlAdapter = new WpfControl(this, renderHandler);
            
            var popupRenderHandler = new WpfRenderHandler(popupImage, _logger);
            var popupAdapter = new WpfPopup(popup, popupRenderHandler);

            _adapter = new CommonBrowserAdapter(nameof(WpfCefBrowser), controlAdapter, popupAdapter, _logger);

            _disposables = new IDisposable[] { controlAdapter, popupAdapter };

            KeyboardNavigation.SetAcceptsReturn(this, true);
        }

        #region Disposable

        ~WpfCefBrowser()
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
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _adapter.Dispose(disposing);
        }

        #endregion

        public event LoadStartEventHandler LoadStart { add => _adapter.LoadStart += value; remove => _adapter.LoadStart -= value; }
        public event LoadEndEventHandler LoadEnd { add => _adapter.LoadEnd += value; remove => _adapter.LoadEnd -= value; }
        public event LoadingStateChangeEventHandler LoadingStateChange { add => _adapter.LoadingStateChange += value; remove => _adapter.LoadingStateChange -= value; }
        public event LoadErrorEventHandler LoadError { add => _adapter.LoadError += value; remove => _adapter.LoadError -= value; }

        public string StartUrl { get => _adapter.StartUrl; set => _adapter.StartUrl = value; }

        public bool AllowsTransparency { get => _adapter.AllowsTransparency; set => _adapter.AllowsTransparency = value; }

        public void NavigateTo(string url) {
            _adapter.NavigateTo(url);
        }

        public void LoadString(string content, string url) {
            _adapter.LoadString(content, url);
        }

        public bool CanGoBack() {
            return _adapter.CanGoBack();
        }

        public void GoBack() {
            _adapter.GoBack();
        }

        public bool CanGoForward() {
            return _adapter.CanGoForward();
        }

        public void GoForward() {
            _adapter.GoForward();
        }

        public void Refresh() {
            _adapter.Refresh();
        }

        public void ExecuteJavaScript(string code, string url = null, int line = 1) {
            _adapter.ExecuteJavaScript(code, url ?? "about:blank", line);
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url = null, int line = 1) {
            return _adapter.EvaluateJavaScript<T>(code, url ?? "about:blank", line);
        }

        public void ShowDeveloperTools() {
            _adapter.ShowDeveloperTools();
        }

        public void RegisterJavascriptObject(object targetObject, string name) {
            _adapter.RegisterJavascriptObject(targetObject, name);
        }

        protected override Size ArrangeOverride(Size arrangeBounds) {
            var size = base.ArrangeOverride(arrangeBounds);
            _adapter.CreateOrUpdateBrowser((int)size.Width, (int)size.Height);

            return size;
        }

        private static Image CreateImage()
        {
            var image = new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

            return image;
        }
    }
}
