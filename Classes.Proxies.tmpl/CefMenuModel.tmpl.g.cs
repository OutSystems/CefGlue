namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Supports creation and modification of menus. See cef_menu_id_t for the
    /// command ids that have default implementations. All user-defined command ids
    /// should be between MENU_ID_USER_FIRST and MENU_ID_USER_LAST. The methods of
    /// this class can only be accessed on the browser process the UI thread.
    /// </summary>
    public sealed unsafe partial class CefMenuModel
    {
        /// <summary>
        /// Create a new MenuModel with the specified |delegate|.
        /// </summary>
        public static cef_menu_model_t* CreateMenuModel(cef_menu_model_delegate_t* @delegate)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.CreateMenuModel
        }
        
        /// <summary>
        /// Returns true if this menu is a submenu.
        /// </summary>
        public int IsSubMenu()
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.IsSubMenu
        }
        
        /// <summary>
        /// Clears the menu. Returns true on success.
        /// </summary>
        public int Clear()
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.Clear
        }
        
        /// <summary>
        /// Returns the number of items in this menu.
        /// </summary>
        public UIntPtr GetCount()
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetCount
        }
        
        /// <summary>
        /// Add a separator to the menu. Returns true on success.
        /// </summary>
        public int AddSeparator()
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.AddSeparator
        }
        
        /// <summary>
        /// Add an item to the menu. Returns true on success.
        /// </summary>
        public int AddItem(int command_id, cef_string_t* label)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.AddItem
        }
        
        /// <summary>
        /// Add a check item to the menu. Returns true on success.
        /// </summary>
        public int AddCheckItem(int command_id, cef_string_t* label)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.AddCheckItem
        }
        
        /// <summary>
        /// Add a radio item to the menu. Only a single item with the specified
        /// |group_id| can be checked at a time. Returns true on success.
        /// </summary>
        public int AddRadioItem(int command_id, cef_string_t* label, int group_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.AddRadioItem
        }
        
        /// <summary>
        /// Add a sub-menu to the menu. The new sub-menu is returned.
        /// </summary>
        public cef_menu_model_t* AddSubMenu(int command_id, cef_string_t* label)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.AddSubMenu
        }
        
        /// <summary>
        /// Insert a separator in the menu at the specified |index|. Returns true on
        /// success.
        /// </summary>
        public int InsertSeparatorAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.InsertSeparatorAt
        }
        
        /// <summary>
        /// Insert an item in the menu at the specified |index|. Returns true on
        /// success.
        /// </summary>
        public int InsertItemAt(UIntPtr index, int command_id, cef_string_t* label)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.InsertItemAt
        }
        
        /// <summary>
        /// Insert a check item in the menu at the specified |index|. Returns true on
        /// success.
        /// </summary>
        public int InsertCheckItemAt(UIntPtr index, int command_id, cef_string_t* label)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.InsertCheckItemAt
        }
        
        /// <summary>
        /// Insert a radio item in the menu at the specified |index|. Only a single
        /// item with the specified |group_id| can be checked at a time. Returns true
        /// on success.
        /// </summary>
        public int InsertRadioItemAt(UIntPtr index, int command_id, cef_string_t* label, int group_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.InsertRadioItemAt
        }
        
        /// <summary>
        /// Insert a sub-menu in the menu at the specified |index|. The new sub-menu
        /// is returned.
        /// </summary>
        public cef_menu_model_t* InsertSubMenuAt(UIntPtr index, int command_id, cef_string_t* label)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.InsertSubMenuAt
        }
        
        /// <summary>
        /// Removes the item with the specified |command_id|. Returns true on success.
        /// </summary>
        public int Remove(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.Remove
        }
        
        /// <summary>
        /// Removes the item at the specified |index|. Returns true on success.
        /// </summary>
        public int RemoveAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.RemoveAt
        }
        
        /// <summary>
        /// Returns the index associated with the specified |command_id| or -1 if not
        /// found due to the command id not existing in the menu.
        /// </summary>
        public int GetIndexOf(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetIndexOf
        }
        
        /// <summary>
        /// Returns the command id at the specified |index| or -1 if not found due to
        /// invalid range or the index being a separator.
        /// </summary>
        public int GetCommandIdAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetCommandIdAt
        }
        
        /// <summary>
        /// Sets the command id at the specified |index|. Returns true on success.
        /// </summary>
        public int SetCommandIdAt(UIntPtr index, int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetCommandIdAt
        }
        
        /// <summary>
        /// Returns the label for the specified |command_id| or empty if not found.
        /// </summary>
        public cef_string_userfree* GetLabel(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetLabel
        }
        
        /// <summary>
        /// Returns the label at the specified |index| or empty if not found due to
        /// invalid range or the index being a separator.
        /// </summary>
        public cef_string_userfree* GetLabelAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetLabelAt
        }
        
        /// <summary>
        /// Sets the label for the specified |command_id|. Returns true on success.
        /// </summary>
        public int SetLabel(int command_id, cef_string_t* label)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetLabel
        }
        
        /// <summary>
        /// Set the label at the specified |index|. Returns true on success.
        /// </summary>
        public int SetLabelAt(UIntPtr index, cef_string_t* label)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetLabelAt
        }
        
        /// <summary>
        /// Returns the item type for the specified |command_id|.
        /// </summary>
        public CefMenuItemType GetType(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetType
        }
        
        /// <summary>
        /// Returns the item type at the specified |index|.
        /// </summary>
        public CefMenuItemType GetTypeAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetTypeAt
        }
        
        /// <summary>
        /// Returns the group id for the specified |command_id| or -1 if invalid.
        /// </summary>
        public int GetGroupId(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetGroupId
        }
        
        /// <summary>
        /// Returns the group id at the specified |index| or -1 if invalid.
        /// </summary>
        public int GetGroupIdAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetGroupIdAt
        }
        
        /// <summary>
        /// Sets the group id for the specified |command_id|. Returns true on success.
        /// </summary>
        public int SetGroupId(int command_id, int group_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetGroupId
        }
        
        /// <summary>
        /// Sets the group id at the specified |index|. Returns true on success.
        /// </summary>
        public int SetGroupIdAt(UIntPtr index, int group_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetGroupIdAt
        }
        
        /// <summary>
        /// Returns the submenu for the specified |command_id| or empty if invalid.
        /// </summary>
        public cef_menu_model_t* GetSubMenu(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetSubMenu
        }
        
        /// <summary>
        /// Returns the submenu at the specified |index| or empty if invalid.
        /// </summary>
        public cef_menu_model_t* GetSubMenuAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetSubMenuAt
        }
        
        /// <summary>
        /// Returns true if the specified |command_id| is visible.
        /// </summary>
        public int IsVisible(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.IsVisible
        }
        
        /// <summary>
        /// Returns true if the specified |index| is visible.
        /// </summary>
        public int IsVisibleAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.IsVisibleAt
        }
        
        /// <summary>
        /// Change the visibility of the specified |command_id|. Returns true on
        /// success.
        /// </summary>
        public int SetVisible(int command_id, int visible)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetVisible
        }
        
        /// <summary>
        /// Change the visibility at the specified |index|. Returns true on success.
        /// </summary>
        public int SetVisibleAt(UIntPtr index, int visible)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetVisibleAt
        }
        
        /// <summary>
        /// Returns true if the specified |command_id| is enabled.
        /// </summary>
        public int IsEnabled(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.IsEnabled
        }
        
        /// <summary>
        /// Returns true if the specified |index| is enabled.
        /// </summary>
        public int IsEnabledAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.IsEnabledAt
        }
        
        /// <summary>
        /// Change the enabled status of the specified |command_id|. Returns true on
        /// success.
        /// </summary>
        public int SetEnabled(int command_id, int enabled)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetEnabled
        }
        
        /// <summary>
        /// Change the enabled status at the specified |index|. Returns true on
        /// success.
        /// </summary>
        public int SetEnabledAt(UIntPtr index, int enabled)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetEnabledAt
        }
        
        /// <summary>
        /// Returns true if the specified |command_id| is checked. Only applies to
        /// check and radio items.
        /// </summary>
        public int IsChecked(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.IsChecked
        }
        
        /// <summary>
        /// Returns true if the specified |index| is checked. Only applies to check
        /// and radio items.
        /// </summary>
        public int IsCheckedAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.IsCheckedAt
        }
        
        /// <summary>
        /// Check the specified |command_id|. Only applies to check and radio items.
        /// Returns true on success.
        /// </summary>
        public int SetChecked(int command_id, int @checked)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetChecked
        }
        
        /// <summary>
        /// Check the specified |index|. Only applies to check and radio items.
        /// Returns true on success.
        /// </summary>
        public int SetCheckedAt(UIntPtr index, int @checked)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetCheckedAt
        }
        
        /// <summary>
        /// Returns true if the specified |command_id| has a keyboard accelerator
        /// assigned.
        /// </summary>
        public int HasAccelerator(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.HasAccelerator
        }
        
        /// <summary>
        /// Returns true if the specified |index| has a keyboard accelerator assigned.
        /// </summary>
        public int HasAcceleratorAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.HasAcceleratorAt
        }
        
        /// <summary>
        /// Set the keyboard accelerator for the specified |command_id|. |key_code|
        /// can be any virtual key or character value. Returns true on success.
        /// </summary>
        public int SetAccelerator(int command_id, int key_code, int shift_pressed, int ctrl_pressed, int alt_pressed)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetAccelerator
        }
        
        /// <summary>
        /// Set the keyboard accelerator at the specified |index|. |key_code| can be
        /// any virtual key or character value. Returns true on success.
        /// </summary>
        public int SetAcceleratorAt(UIntPtr index, int key_code, int shift_pressed, int ctrl_pressed, int alt_pressed)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetAcceleratorAt
        }
        
        /// <summary>
        /// Remove the keyboard accelerator for the specified |command_id|. Returns
        /// true on success.
        /// </summary>
        public int RemoveAccelerator(int command_id)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.RemoveAccelerator
        }
        
        /// <summary>
        /// Remove the keyboard accelerator at the specified |index|. Returns true on
        /// success.
        /// </summary>
        public int RemoveAcceleratorAt(UIntPtr index)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.RemoveAcceleratorAt
        }
        
        /// <summary>
        /// Retrieves the keyboard accelerator for the specified |command_id|. Returns
        /// true on success.
        /// </summary>
        public int GetAccelerator(int command_id, int* key_code, int* shift_pressed, int* ctrl_pressed, int* alt_pressed)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetAccelerator
        }
        
        /// <summary>
        /// Retrieves the keyboard accelerator for the specified |index|. Returns true
        /// on success.
        /// </summary>
        public int GetAcceleratorAt(UIntPtr index, int* key_code, int* shift_pressed, int* ctrl_pressed, int* alt_pressed)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetAcceleratorAt
        }
        
        /// <summary>
        /// Set the explicit color for |command_id| and |color_type| to |color|.
        /// Specify a |color| value of 0 to remove the explicit color. If no explicit
        /// color or default color is set for |color_type| then the system color will
        /// be used. Returns true on success.
        /// </summary>
        public int SetColor(int command_id, CefMenuColorType color_type, uint color)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetColor
        }
        
        /// <summary>
        /// Set the explicit color for |command_id| and |index| to |color|. Specify a
        /// |color| value of 0 to remove the explicit color. Specify an |index| value
        /// of -1 to set the default color for items that do not have an explicit
        /// color set. If no explicit color or default color is set for |color_type|
        /// then the system color will be used. Returns true on success.
        /// </summary>
        public int SetColorAt(int index, CefMenuColorType color_type, uint color)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetColorAt
        }
        
        /// <summary>
        /// Returns in |color| the color that was explicitly set for |command_id| and
        /// |color_type|. If a color was not set then 0 will be returned in |color|.
        /// Returns true on success.
        /// </summary>
        public int GetColor(int command_id, CefMenuColorType color_type, uint* color)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetColor
        }
        
        /// <summary>
        /// Returns in |color| the color that was explicitly set for |command_id| and
        /// |color_type|. Specify an |index| value of -1 to return the default color
        /// in |color|. If a color was not set then 0 will be returned in |color|.
        /// Returns true on success.
        /// </summary>
        public int GetColorAt(int index, CefMenuColorType color_type, uint* color)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.GetColorAt
        }
        
        /// <summary>
        /// Sets the font list for the specified |command_id|. If |font_list| is empty
        /// the system font will be used. Returns true on success. The format is
        /// "&lt;FONT_FAMILY_LIST&gt;,[STYLES] &lt;SIZE&gt;", where:
        /// - FONT_FAMILY_LIST is a comma-separated list of font family names,
        /// - STYLES is an optional space-separated list of style names
        /// (case-sensitive "Bold" and "Italic" are supported), and
        /// - SIZE is an integer font size in pixels with the suffix "px".
        /// Here are examples of valid font description strings:
        /// - "Arial, Helvetica, Bold Italic 14px"
        /// - "Arial, 14px"
        /// </summary>
        public int SetFontList(int command_id, cef_string_t* font_list)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetFontList
        }
        
        /// <summary>
        /// Sets the font list for the specified |index|. Specify an |index| value of
        /// -1 to set the default font. If |font_list| is empty the system font will
        /// be used. Returns true on success. The format is
        /// "&lt;FONT_FAMILY_LIST&gt;,[STYLES] &lt;SIZE&gt;", where:
        /// - FONT_FAMILY_LIST is a comma-separated list of font family names,
        /// - STYLES is an optional space-separated list of style names
        /// (case-sensitive "Bold" and "Italic" are supported), and
        /// - SIZE is an integer font size in pixels with the suffix "px".
        /// Here are examples of valid font description strings:
        /// - "Arial, Helvetica, Bold Italic 14px"
        /// - "Arial, 14px"
        /// </summary>
        public int SetFontListAt(int index, cef_string_t* font_list)
        {
            throw new NotImplementedException(); // TODO: CefMenuModel.SetFontListAt
        }
        
    }
}
