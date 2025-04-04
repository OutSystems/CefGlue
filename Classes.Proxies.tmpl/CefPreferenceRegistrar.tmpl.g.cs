namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that manages custom preference registrations.
    /// </summary>
    public sealed unsafe partial class CefPreferenceRegistrar
    {
        /// <summary>
        /// Register a preference with the specified |name| and |default_value|. To
        /// avoid conflicts with built-in preferences the |name| value should contain
        /// an application-specific prefix followed by a period (e.g. "myapp.value").
        /// The contents of |default_value| will be copied. The data type for the
        /// preference will be inferred from |default_value|'s type and cannot be
        /// changed after registration. Returns true on success. Returns false if
        /// |name| is already registered or if |default_value| has an invalid type.
        /// This method must be called from within the scope of the
        /// CefBrowserProcessHandler::OnRegisterCustomPreferences callback.
        /// </summary>
        public int AddPreference(cef_string_t* name, cef_value_t* default_value)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceRegistrar.AddPreference
        }
        
    }
}
