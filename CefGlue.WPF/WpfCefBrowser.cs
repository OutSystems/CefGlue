﻿using System;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Platform;
using Xilium.CefGlue.WPF.Platform;

namespace Xilium.CefGlue.WPF
{
    /// <summary>
    /// The WPF CEF browser.
    /// </summary>
    public class WpfCefBrowser : BaseCefBrowser
    {
        public WpfCefBrowser()
            : base(null)
        {
            KeyboardNavigation.SetAcceptsReturn(this, true);
        }

        public WpfCefBrowser(Func<CefRequestContext> cefRequestContextFactory = null) : base(cefRequestContextFactory) { }

        internal override IControl CreateControl()
        {
            return new WpfControl(this);
        }

        internal override IOffScreenControlHost CreateOffScreenControlHost()
        {
            return new WpfOffScreenControlHost(this);
        }

        internal override IOffScreenPopupHost CreatePopupHost()
        {
            var popup = new Popup
            {
                PlacementTarget = this,
                Placement = PlacementMode.Relative,
            };

            return new WpfPopup(popup);
        }
    }
}
