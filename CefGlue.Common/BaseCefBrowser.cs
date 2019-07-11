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

        /// <summary>
        /// Creates an instance of the browser adapter.
        /// </summary>
        /// <returns></returns>
        internal abstract CommonBrowserAdapter CreateAdapter();

        /// <summary>
        /// Event fired when the browser starts loading a frame.
        /// </summary>
        public event LoadStartEventHandler LoadStart { add => _adapter.LoadStart += value; remove => _adapter.LoadStart -= value; }

        /// <summary>
        /// Event fired when the browser ends loading a frame.
        /// </summary>
        public event LoadEndEventHandler LoadEnd { add => _adapter.LoadEnd += value; remove => _adapter.LoadEnd -= value; }

        /// <summary>
        /// Event fired when the loading state of a frame changes.
        /// </summary>
        public event LoadingStateChangeEventHandler LoadingStateChange { add => _adapter.LoadingStateChange += value; remove => _adapter.LoadingStateChange -= value; }

        /// <summary>
        /// Event fired when the an error occurs while loading a frame.
        /// </summary>
        public event LoadErrorEventHandler LoadError { add => _adapter.LoadError += value; remove => _adapter.LoadError -= value; }

        /// <summary>
        /// Event fired when the address changes.
        /// </summary>
        public event AddressChangedEventHandler AddressChanged { add => _adapter.AddressChanged += value; remove => _adapter.AddressChanged -= value; }

        /// <summary>
        /// Event fired when the page title changes.
        /// </summary>
        public event ConsoleMessageEventHandler ConsoleMessage { add => _adapter.ConsoleMessage += value; remove => _adapter.ConsoleMessage -= value; }

        /// <summary>
        /// Event fired when the browser receives a status message. |message| contains the text that will be displayed in the status message.
        /// </summary>
        public event StatusMessageEventHandler StatusMessage { add => _adapter.StatusMessage += value; remove => _adapter.StatusMessage -= value; }

        /// <summary>
        /// Event fired when the title changes.
        /// </summary>
        public event TitleChangedEventHandler TitleChanged { add => _adapter.TitleChanged += value; remove => _adapter.TitleChanged -= value; }

        /// <summary>
        /// Return the handler for context menus. If no handler is provided the default implementation will be used.
        /// </summary>
        public ContextMenuHandler ContextMenuHandler { get => _adapter.ContextMenuHandler; set => _adapter.ContextMenuHandler = value; }

        /// <summary>
        /// Return the handler for dialogs. If no handler is provided the default implementation will be used.
        /// </summary>
        public DialogHandler DialogHandler { get => _adapter.DialogHandler; set => _adapter.DialogHandler = value; }

        /// <summary>
        /// Return the handler for download events. If no handler is returned downloads will not be allowed.
        /// </summary>
        public DownloadHandler DownloadHandler { get => _adapter.DownloadHandler; set => _adapter.DownloadHandler = value; }

        /// <summary>
        /// Return the handler for drag events.
        /// </summary>
        public DragHandler DragHandler { get => _adapter.DragHandler; set => _adapter.DragHandler = value; }

        /// <summary>
        /// Return the handler for find result events.
        /// </summary>
        public FindHandler FindHandler { get => _adapter.FindHandler; set => _adapter.FindHandler = value; }

        /// <summary>
        /// Return the handler for focus events.
        /// </summary>
        public FocusHandler FocusHandler { get => _adapter.FocusHandler; set => _adapter.FocusHandler = value; }

        /// <summary>
        /// Return the handler for keyboard events.
        /// </summary>
        public KeyboardHandler KeyboardHandler { get => _adapter.KeyboardHandler; set => _adapter.KeyboardHandler = value; }

        /// <summary>
        /// Return the handler for browser request events.
        /// </summary>
        public RequestHandler RequestHandler { get => _adapter.RequestHandler; set => _adapter.RequestHandler = value; }

        /// <summary>
        /// Return the handler for browser life span events.
        /// </summary>
        public LifeSpanHandler LifeSpanHandler { get => _adapter.LifeSpanHandler; set => _adapter.LifeSpanHandler = value; }

        /// <summary>
        /// Return the handler for browser display state events.
        /// </summary>
        public DisplayHandler DisplayHandler { get => _adapter.DisplayHandler; set => _adapter.DisplayHandler = value; }

        /// <summary>
        /// Return the handler for off-screen rendering events.
        /// </summary>
        public RenderHandler RenderHandler { get => _adapter.RenderHandler; set => _adapter.RenderHandler = value; }

        /// <summary>
        /// Return the handler for JavaScript dialogs. If no handler is provided the default implementation will be used.
        /// </summary>
        public JSDialogHandler JSDialogHandler { get => _adapter.JSDialogHandler; set => _adapter.JSDialogHandler = value; }

        /// <summary>
        /// Initial url.
        /// </summary>
        public string StartUrl { get => _adapter.StartUrl; set => _adapter.StartUrl = value; }

        /// <summary>
        /// Allows the browser background to be transparent.
        /// </summary>
        public bool AllowsTransparency { get => _adapter.AllowsTransparency; set => _adapter.AllowsTransparency = value; }

        /// <summary>
        /// Returns true when the underlying browser has been initialized.
        /// </summary>
        public bool IsBrowserInitialized => _adapter.IsInitialized;

        /// <summary>
        /// Returns true if the browser is currently loading.
        /// </summary>
        public bool IsLoading => _adapter.IsLoading;

        /// <summary>
        /// Returns the current page title.
        /// </summary>
        public string Title => _adapter.Title;

        /// <summary>
        /// Get or set the current zoom level. The default zoom level is 0.0.
        /// </summary>
        public double ZoomLevel
        {
            get => _adapter.ZoomLevel;
            set => _adapter.ZoomLevel = value;
        }

        /// <summary>
        /// Load the specified |url|.
        /// </summary>
        public void NavigateTo(string url) {
            _adapter.NavigateTo(url);
        }

        /// <summary>
        /// Load the contents specified with the specified dummy |url|. |url|
        /// should have a standard scheme (for example, http scheme) or behaviors like
        /// link clicks and web security restrictions may not behave as expected.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="url"></param>
        public void LoadString(string content, string url) {
            _adapter.LoadString(content, url);
        }

        /// <summary>
        /// Returns true if the browser can navigate backwards.
        /// </summary>
        /// <returns></returns>
        public bool CanGoBack => _adapter.CanGoBack();

        /// <summary>
        /// Navigate backwards.
        /// </summary>
        public void GoBack() {
            _adapter.GoBack();
        }

        /// <summary>
        /// Returns true if the browser can navigate forward.
        /// </summary>
        /// <returns></returns>
        public bool CanGoForward => _adapter.CanGoForward();

        /// <summary>
        /// Navigate forwards.
        /// </summary>
        public void GoForward()
        {
            _adapter.GoForward();
        }

        /// <summary>
        /// Reload the current page.
        /// </summary>
        public void Reload(bool ignoreCache = false) {
            _adapter.Reload(ignoreCache);
        }

        /// <summary>
        /// Executes the specified javascript snippet.
        /// </summary>
        /// <param name="code">The javascript snippet.</param>
        /// <param name="url">Url where the script in question can be found.</param>
        /// <param name="line">The base line number to use for error, if any.</param>
        public void ExecuteJavaScript(string code, string url = null, int line = 1) {
            _adapter.ExecuteJavaScript(code, url ?? "about:blank", line);
        }

        /// <summary>
        /// Evaluates the specified javascript snippet.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="code">The javascript snippet.</param>
        /// <param name="url">Url where the script in question can be found.</param>
        /// <param name="line">The base line number to use for error, if any.</param>
        /// <returns>The result of the evaluation.</returns>
        public Task<T> EvaluateJavaScript<T>(string code, string url = null, int line = 1) {
            return _adapter.EvaluateJavaScript<T>(code, url ?? "about:blank", line);
        }

        /// <summary>
        /// Opens the Developer tools.
        /// </summary>
        public void ShowDeveloperTools() {
            _adapter.ShowDeveloperTools();
        }

        /// <summary>
        /// Closes the Developer tools (opened previously).
        /// </summary>
        public void CloseDeveloperTools()
        {
            _adapter.CloseDeveloperTools();
        }

        /// <summary>
        /// Registers a Javascript object in this browser instance.
        /// </summary>
        /// <param name="targetObject">The object to be made accessible to Javascript</param>
        /// <param name="name">The name of the object. (e.g. "potatoes", if you want the object to be accessible as window.potatoes).</param>
        public void RegisterJavascriptObject(object targetObject, string name) {
            _adapter.RegisterJavascriptObject(targetObject, name);
        }

        protected void CreateOrUpdateBrowser(int width, int height)
        {
            _adapter.CreateOrUpdateBrowser(width, height);
        }
    }
}
