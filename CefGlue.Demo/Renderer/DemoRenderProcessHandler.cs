namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;

    class DemoRenderProcessHandler : CefRenderProcessHandler
    {
        protected override bool OnBeforeNavigation(CefBrowser browser, CefFrame frame, CefRequest request, CefNavigationType navigationType, bool isRedirect)
        {
            Console.WriteLine("OnBeforeNavigation: Request.Url={0} NavigationType={1} IsRedirect={2}",
                request.Url,
                navigationType,
                isRedirect
                );

            return false;
        }
    }
}
