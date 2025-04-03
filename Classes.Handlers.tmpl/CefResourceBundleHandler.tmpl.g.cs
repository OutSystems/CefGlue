namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to implement a custom resource bundle interface. See CefSettings
    /// for additional options related to resource bundle loading. The methods of
    /// this class may be called on multiple threads.
    /// </summary>
    public abstract unsafe partial class CefResourceBundleHandler
    {
        private int get_localized_string(cef_resource_bundle_handler_t* self, int string_id, cef_string_t* @string)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceBundleHandler.GetLocalizedString
        }
        
        /// <summary>
        /// Called to retrieve a localized translation for the specified |string_id|.
        /// To provide the translation set |string| to the translation string and
        /// return true. To use the default translation return false. Use the
        /// cef_id_for_pack_string_name() function for version-safe mapping of string
        /// IDS names from cef_pack_strings.h to version-specific numerical
        /// |string_id| values.
        /// </summary>
        // protected abstract int GetLocalizedString(int string_id, cef_string_t* @string);
        
        private int get_data_resource(cef_resource_bundle_handler_t* self, int resource_id, void** data, UIntPtr* data_size)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceBundleHandler.GetDataResource
        }
        
        /// <summary>
        /// Called to retrieve data for the specified scale independent |resource_id|.
        /// To provide the resource data set |data| and |data_size| to the data
        /// pointer and size respectively and return true. To use the default resource
        /// data return false. The resource data will not be copied and must remain
        /// resident in memory. Use the cef_id_for_pack_resource_name() function for
        /// version-safe mapping of resource IDR names from cef_pack_resources.h to
        /// version-specific numerical |resource_id| values.
        /// </summary>
        // protected abstract int GetDataResource(int resource_id, void** data, UIntPtr* data_size);
        
        private int get_data_resource_for_scale(cef_resource_bundle_handler_t* self, int resource_id, CefScaleFactor scale_factor, void** data, UIntPtr* data_size)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceBundleHandler.GetDataResourceForScale
        }
        
        /// <summary>
        /// Called to retrieve data for the specified |resource_id| nearest the scale
        /// factor |scale_factor|. To provide the resource data set |data| and
        /// |data_size| to the data pointer and size respectively and return true. To
        /// use the default resource data return false. The resource data will not be
        /// copied and must remain resident in memory. Use the
        /// cef_id_for_pack_resource_name() function for version-safe mapping of
        /// resource IDR names from cef_pack_resources.h to version-specific numerical
        /// |resource_id| values.
        /// </summary>
        // protected abstract int GetDataResourceForScale(int resource_id, CefScaleFactor scale_factor, void** data, UIntPtr* data_size);
        
    }
}
