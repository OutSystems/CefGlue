namespace Xilium.CefGlue.Common.Platform
{
    internal abstract class BaseBrowserSurface
    {
        protected CefBrowserHost _browserHost;

        public virtual void SetBrowserHost(CefBrowserHost browserHost)
        {
            _browserHost = browserHost;
        }

        public abstract void Show();

        public abstract void Hide();

        public abstract bool MoveAndResize(int x, int y, int width, int height);

        public abstract CefRectangle GetViewRect();
    }
}
