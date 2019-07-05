using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Xilium.CefGlue.Avalonia.Platform;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Avalonia
{
    public class AvaloniaCefBrowser : TemplatedControl, IDisposable
    {
        private readonly ILogger _logger;
        private readonly CommonBrowserAdapter _adapter;

        private readonly Popup _popup;
        private readonly Image _popupImage;

        private Image _image;

        public AvaloniaCefBrowser()
        {
            _logger = new NLogLogger(nameof(AvaloniaCefBrowser));

            // TODO
           //_popupImage = new Image()
           //{
           //    Stretch = Stretch.None,
           //    HorizontalAlignment = HorizontalAlignment.Left,
           //    VerticalAlignment = VerticalAlignment.Top,
           //    Source = _popupBitmap
           //};

            _popup = new Popup
            {
                Child = _popupImage,
                PlacementTarget = this,
                PlacementMode = PlacementMode.Bottom
            };

            _adapter = new CommonBrowserAdapter(nameof(AvaloniaCefBrowser), new AvaloniaControl(this), new AvaloniaPopup(_popup), _logger);
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
            _adapter.RenderHandler?.Dispose();
            _adapter.PopupRenderHandler?.Dispose();
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

        public void ExecuteJavaScript(string code, string url = null, int line = 1)
        {
            _adapter.ExecuteJavaScript(code, url ?? "about:blank", line);
        }

        public Task<T> EvaluateJavaScript<T>(string code, string url = null, int line = 1)
        {
            return _adapter.EvaluateJavaScript<T>(code, url ?? "about:blank", line);
        }

        public void ShowDeveloperTools()
        {
            _adapter.ShowDeveloperTools();
        }

        public void RegisterJavascriptObject(object targetObject, string name)
        {
            _adapter.RegisterJavascriptObject(targetObject, name);
        }

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);
            _image = e.NameScope.Find<Image>("PART_Image");
            _adapter.RenderHandler?.Dispose();
            _adapter.RenderHandler = new AvaloniaRenderHandler(_image, _logger);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);

            if (_image != null)
            {
                _adapter.CreateOrUpdateBrowser((int) size.Width, (int) size.Height);
            }

            return size;
        }

        protected void OnPopupSizeChanged(CefRectangle rect)
        {
            // TODO
            //_mainUiDispatcher.Post(
            //    () =>
            //    {
            //        _popupRenderHandler.Dispose();
            //        _popupRenderHandler = new RenderHandler(_popupImage, _logger);

            //        _popupBitmap = new WriteableBitmap(new PixelSize(rect.Width, rect.Height), new Vector(96, 96), PixelFormat.Bgra8888);

            //        _popupImage.Source = _popupBitmap;

            //        _popup.Width = rect.Width;
            //        _popup.Height = rect.Height;
            //        _popup.HorizontalOffset = rect.X;
            //        _popup.VerticalOffset = rect.Y;
            //    });
        }
    }
}
