namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IMainView : IDisposable
    {
        void NewTab(string url);
        void Close();

        void NavigateTo(string url);
    }
}
