namespace Xilium.CefGlue.Demo.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Xilium.CefGlue;

    internal sealed class WebKeyboardHandler : CefKeyboardHandler
    {
        private readonly WebBrowser _core;

        public WebKeyboardHandler(WebBrowser core)
        {
            _core = core;
        }

        protected override bool OnKeyEvent(CefBrowser browser, CefKeyEvent keyEvent, IntPtr osEvent)
        {
            Debug.Print($"{nameof(WebKeyboardHandler)}::{nameof(OnKeyEvent)}: NativeKeyCode={keyEvent.NativeKeyCode} FocusOnEditableField={keyEvent.FocusOnEditableField}");
            return base.OnKeyEvent(browser, keyEvent, osEvent);
        }
    }
}
