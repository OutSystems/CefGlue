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

        private readonly Popup _popup;
        private readonly Image _popupImage;

        private Image _image;

        public WpfCefBrowser() {
            _logger = new Logger(nameof(WpfCefBrowser));

            _popupImage = new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                // TODO Source = _popupBitmap
            };

            RenderOptions.SetBitmapScalingMode(_popupImage, BitmapScalingMode.NearestNeighbor);

            _popup = new Popup
            {
                Child = _popupImage,
                PlacementTarget = this,
                Placement = PlacementMode.Relative
            };

            var control = new WpfControl(this);
            var popup = new WpfPopup(_popup);
            _adapter = new CommonBrowserAdapter(nameof(WpfCefBrowser), control, popup, _logger);

            _disposables = new IDisposable[] { control, popup };

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


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _image = new Image()
            {
                Focusable = false,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stretch = Stretch.None
            };

            Content = _image;

            _adapter.RenderHandler?.Dispose();
            _adapter.RenderHandler = new WpfRenderHandler(_image, _logger);
        }

        protected override Size ArrangeOverride(Size arrangeBounds) {
            var size = base.ArrangeOverride(arrangeBounds);

            if (_image != null) {
                _adapter.CreateOrUpdateBrowser((int)size.Width, (int)size.Height);
            }

            return size;
        }

        // TODO
        //protected override void OnPopupSizeChanged(CefRectangle rect)
        //{
        //    _mainUiDispatcher.Invoke(
        //        new Action(
        //            () =>
        //            {
        //                _popupBitmap = null;
        //                _popupBitmap = new WriteableBitmap(rect.Width, rect.Height, 96, 96, PixelFormats.Bgr32, null);

        //                _popupImage.Source = _popupBitmap;

        //                _popup.Width = rect.Width;
        //                _popup.Height = rect.Height;
        //                _popup.HorizontalOffset = rect.X;
        //                _popup.VerticalOffset = rect.Y;
        //            }));
        //}
    }
}
