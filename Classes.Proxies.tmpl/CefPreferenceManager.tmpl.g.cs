namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Manage access to preferences. Many built-in preferences are registered by
    /// Chromium. Custom preferences can be registered in
    /// CefBrowserProcessHandler::OnRegisterCustomPreferences.
    /// </summary>
    public sealed unsafe partial class CefPreferenceManager
    {
        /// <summary>
        /// Returns the current Chrome Variations configuration (combination of field
        /// trials and chrome://flags) as equivalent command-line switches
        /// (`--[enable|disable]-features=XXXX`, etc). These switches can be used to
        /// apply the same configuration when launching a CEF-based application. See
        /// https://developer.chrome.com/docs/web-platform/chrome-variations for
        /// background and details. Note that field trial tests are disabled by
        /// default in Official CEF builds (via the
        /// `disable_fieldtrial_testing_config=true` GN flag). This method must be
        /// called on the browser process UI thread.
        /// </summary>
        public static void GetChromeVariationsAsSwitches(cef_string_list* switches)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.GetChromeVariationsAsSwitches
        }
        
        /// <summary>
        /// Returns the current Chrome Variations configuration (combination of field
        /// trials and chrome://flags) as human-readable strings. This is the
        /// human-readable equivalent of the "Active Variations" section of
        /// chrome://version. See
        /// https://developer.chrome.com/docs/web-platform/chrome-variations for
        /// background and details. Note that field trial tests are disabled by
        /// default in Official CEF builds (via the
        /// `disable_fieldtrial_testing_config=true` GN flag). This method must be
        /// called on the browser process UI thread.
        /// </summary>
        public static void GetChromeVariationsAsStrings(cef_string_list* strings)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.GetChromeVariationsAsStrings
        }
        
        /// <summary>
        /// Returns the global preference manager object.
        /// </summary>
        public static cef_preference_manager_t* GetGlobalPreferenceManager()
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.GetGlobalPreferenceManager
        }
        
        /// <summary>
        /// Returns true if a preference with the specified |name| exists. This method
        /// must be called on the browser process UI thread.
        /// </summary>
        public int HasPreference(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.HasPreference
        }
        
        /// <summary>
        /// Returns the value for the preference with the specified |name|. Returns
        /// NULL if the preference does not exist. The returned object contains a copy
        /// of the underlying preference value and modifications to the returned
        /// object will not modify the underlying preference value. This method must
        /// be called on the browser process UI thread.
        /// </summary>
        public cef_value_t* GetPreference(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.GetPreference
        }
        
        /// <summary>
        /// Returns all preferences as a dictionary. If |include_defaults| is true
        /// then preferences currently at their default value will be included. The
        /// returned object contains a copy of the underlying preference values and
        /// modifications to the returned object will not modify the underlying
        /// preference values. This method must be called on the browser process UI
        /// thread.
        /// </summary>
        public cef_dictionary_value_t* GetAllPreferences(int include_defaults)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.GetAllPreferences
        }
        
        /// <summary>
        /// Returns true if the preference with the specified |name| can be modified
        /// using SetPreference. As one example preferences set via the command-line
        /// usually cannot be modified. This method must be called on the browser
        /// process UI thread.
        /// </summary>
        public int CanSetPreference(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.CanSetPreference
        }
        
        /// <summary>
        /// Set the |value| associated with preference |name|. Returns true if the
        /// value is set successfully and false otherwise. If |value| is NULL the
        /// preference will be restored to its default value. If setting the
        /// preference fails then |error| will be populated with a detailed
        /// description of the problem. This method must be called on the browser
        /// process UI thread.
        /// </summary>
        public int SetPreference(cef_string_t* name, cef_value_t* value, cef_string_t* error)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.SetPreference
        }
        
        /// <summary>
        /// Add an observer for preference changes. |name| is the name of the
        /// preference to observe. If |name| is empty then all preferences will
        /// be observed. Observing all preferences has performance consequences and
        /// is not recommended outside of testing scenarios. The observer will remain
        /// registered until the returned Registration object is destroyed. This
        /// method must be called on the browser process UI thread.
        /// </summary>
        public cef_registration_t* AddPreferenceObserver(cef_string_t* name, cef_preference_observer_t* observer)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceManager.AddPreferenceObserver
        }
        
    }
}
