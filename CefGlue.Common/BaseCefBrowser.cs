using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Common
{
    /// <summary>
    /// Browser interface shared among the several implementations (Avalonia, WPF, ...)
    /// </summary>
    public abstract partial class BaseCefBrowser : IDisposable
    {
        protected readonly ILogger _logger;

        private readonly CommonBrowserAdapter _adapter;

        #region Disposable

        public BaseCefBrowser() {
            _logger = new Logger(nameof(BaseCefBrowser));
            _adapter = CreateAdapter();
        }

        ~BaseCefBrowser() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            _adapter.Dispose(disposing);
        }

        #endregion

        internal abstract CommonBrowserAdapter CreateAdapter();

        public event LoadStartEventHandler LoadStart { add => _adapter.LoadStart += value; remove => _adapter.LoadStart -= value; }
        public event LoadEndEventHandler LoadEnd { add => _adapter.LoadEnd += value; remove => _adapter.LoadEnd -= value; }
        public event LoadingStateChangeEventHandler LoadingStateChange { add => _adapter.LoadingStateChange += value; remove => _adapter.LoadingStateChange -= value; }
        public event LoadErrorEventHandler LoadError { add => _adapter.LoadError += value; remove => _adapter.LoadError -= value; }

        public event AddressChangedEventHandler AddressChanged { add => _adapter.AddressChanged += value; remove => _adapter.AddressChanged -= value; }
        public event ConsoleMessageEventHandler ConsoleMessage { add => _adapter.ConsoleMessage += value; remove => _adapter.ConsoleMessage -= value; }
        public event StatusMessageEventHandler StatusMessage { add => _adapter.StatusMessage += value; remove => _adapter.StatusMessage -= value; }
        public event TitleChangedEventHandler TitleChanged { add => _adapter.TitleChanged += value; remove => _adapter.TitleChanged -= value; }

        public ContextMenuHandler ContextMenuHandler { get => _adapter.ContextMenuHandler; set => _adapter.ContextMenuHandler = value; }
        public DialogHandler DialogHandler { get => _adapter.DialogHandler; set => _adapter.DialogHandler = value; }
        public DownloadHandler DownloadHandler { get => _adapter.DownloadHandler; set => _adapter.DownloadHandler = value; }
        public DragHandler DragHandler { get => _adapter.DragHandler; set => _adapter.DragHandler = value; }
        public FindHandler FindHandler { get => _adapter.FindHandler; set => _adapter.FindHandler = value; }
        public FocusHandler FocusHandler { get => _adapter.FocusHandler; set => _adapter.FocusHandler = value; }
        public KeyboardHandler KeyboardHandler { get => _adapter.KeyboardHandler; set => _adapter.KeyboardHandler = value; }
        public RequestHandler RequestHandler { get => _adapter.RequestHandler; set => _adapter.RequestHandler = value; }
        public LifeSpanHandler LifeSpanHandler { get => _adapter.LifeSpanHandler; set => _adapter.LifeSpanHandler = value; }
        public DisplayHandler DisplayHandler { get => _adapter.DisplayHandler; set => _adapter.DisplayHandler = value; }
        public RenderHandler RenderHandler { get => _adapter.RenderHandler; set => _adapter.RenderHandler = value; }
        public JSDialogHandler JSDialogHandler { get => _adapter.JSDialogHandler; set => _adapter.JSDialogHandler = value; }

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

        protected void CreateOrUpdateBrowser(int width, int height)
        {
            _adapter.CreateOrUpdateBrowser(width, height);
        }
    }
}
