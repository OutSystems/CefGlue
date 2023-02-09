using System.Collections.Generic;

namespace Xilium.CefGlue.Common.Helpers
{
    internal class MenuEntry
    {
        public bool IsSeparator { get; set; }

        public string Label { get; set; }

        public bool IsEnabled { get; set; }

        public bool? IsChecked { get; set; }

        public MenuEntry[] SubEntries { get; set; }

        public int CommandId { get; set; }

        internal static MenuEntry[] FromCefModel(CefMenuModel model)
        {
            var menuItems = new List<MenuEntry>();
            for (nuint i = 0; i < model.Count; i++)
            {
                var entry = new MenuEntry()
                {
                    Label = model.GetLabelAt(i) ?? "",
                    IsEnabled = model.IsEnabledAt(i),
                    CommandId = model.GetCommandIdAt(i)
                };

                switch (model.GetItemTypeAt(i))
                {
                    case CefMenuItemType.Separator:
                        entry.IsSeparator = true;
                        break;

                    case CefMenuItemType.Check:
                        entry.IsChecked = model.IsCheckedAt(i);
                        break;
                }

                var subMenuModel = model.GetSubMenuAt(i);
                if (subMenuModel != null)
                {
                    entry.SubEntries = FromCefModel(subMenuModel);
                }

                menuItems.Add(entry);
            }

            return menuItems.ToArray();
        }
    }
}
