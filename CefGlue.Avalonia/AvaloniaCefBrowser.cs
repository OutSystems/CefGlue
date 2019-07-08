using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
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

        public AvaloniaCefBrowser()
        {
            _logger = new Logger(nameof(AvaloniaCefBrowser));

            var image = CreateImage();
            VisualChildren.Add(image);

            var popupImage = CreateImage();
            var popup = new Popup
            {
                PlacementTarget = this,
                PlacementMode = PlacementMode.Bottom,
                Child = popupImage
            };

            var renderHandler = new AvaloniaRenderHandler(image, _logger);
            var controlAdapter = new AvaloniaControl(image, renderHandler); // use the image, otherwise some behaviors won't work as expected (eg: cursors do not change)

            var popupRenderHandler = new AvaloniaRenderHandler(popupImage, _logger);
            var popupAdapter = new AvaloniaPopup(popup, popupRenderHandler);

            _adapter = new CommonBrowserAdapter(nameof(AvaloniaCefBrowser), controlAdapter, popupAdapter, _logger);
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

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            _adapter.CreateOrUpdateBrowser((int) size.Width, (int) size.Height);
            return size;
        }

        private static Image CreateImage()
        {
            return new Image()
            {
                Focusable = false,
                Stretch = Stretch.None,
                HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Left,
                VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Top,
            };
        }
    }
}
