namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to create and/or parse command line arguments. Arguments with
    /// "--", "-" and, on Windows, "/" prefixes are considered switches. Switches
    /// will always precede any arguments without switch prefixes. Switches can
    /// optionally have a value specified using the "=" delimiter (e.g.
    /// "-switch=value"). An argument of "--" will terminate switch parsing with all
    /// subsequent tokens, regardless of prefix, being interpreted as non-switch
    /// arguments. Switch names should be lowercase ASCII and will be converted to
    /// such if necessary. Switch values will retain the original case and UTF8
    /// encoding. This class can be used before CefInitialize() is called.
    /// </summary>
    public sealed unsafe partial class CefCommandLine
    {
        /// <summary>
        /// Create a new CefCommandLine instance.
        /// </summary>
        public static cef_command_line_t* CreateCommandLine()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.CreateCommandLine
        }
        
        /// <summary>
        /// Returns the singleton global CefCommandLine object. The returned object
        /// will be read-only.
        /// </summary>
        public static cef_command_line_t* GetGlobalCommandLine()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.GetGlobalCommandLine
        }
        
        /// <summary>
        /// Returns true if this object is valid. Do not call any other methods if
        /// this function returns false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.IsValid
        }
        
        /// <summary>
        /// Returns true if the values of this object are read-only. Some APIs may
        /// expose read-only objects.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.IsReadOnly
        }
        
        /// <summary>
        /// Returns a writable copy of this object.
        /// </summary>
        public cef_command_line_t* Copy()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.Copy
        }
        
        /// <summary>
        /// Initialize the command line with the specified |argc| and |argv| values.
        /// The first argument must be the name of the program. This method is only
        /// supported on non-Windows platforms.
        /// </summary>
        public void InitFromArgv(int argc, byte** argv)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.InitFromArgv
        }
        
        /// <summary>
        /// Initialize the command line with the string returned by calling
        /// GetCommandLineW(). This method is only supported on Windows.
        /// </summary>
        public void InitFromString(cef_string_t* command_line)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.InitFromString
        }
        
        /// <summary>
        /// Reset the command-line switches and arguments but leave the program
        /// component unchanged.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.Reset
        }
        
        /// <summary>
        /// Retrieve the original command line string as a vector of strings.
        /// The argv array:
        /// `{ program, [(--|-|/)switch[=value]]*, [--], [argument]* }`
        /// </summary>
        public void GetArgv(cef_string_list* argv)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.GetArgv
        }
        
        /// <summary>
        /// Constructs and returns the represented command line string. Use this
        /// method cautiously because quoting behavior is unclear.
        /// </summary>
        public cef_string_userfree* GetCommandLineString()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.GetCommandLineString
        }
        
        /// <summary>
        /// Get the program part of the command line string (the first item).
        /// </summary>
        public cef_string_userfree* GetProgram()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.GetProgram
        }
        
        /// <summary>
        /// Set the program part of the command line string (the first item).
        /// </summary>
        public void SetProgram(cef_string_t* program)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.SetProgram
        }
        
        /// <summary>
        /// Returns true if the command line has switches.
        /// </summary>
        public int HasSwitches()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.HasSwitches
        }
        
        /// <summary>
        /// Returns true if the command line contains the given switch.
        /// </summary>
        public int HasSwitch(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.HasSwitch
        }
        
        /// <summary>
        /// Returns the value associated with the given switch. If the switch has no
        /// value or isn't present this method returns the empty string.
        /// </summary>
        public cef_string_userfree* GetSwitchValue(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.GetSwitchValue
        }
        
        /// <summary>
        /// Returns the map of switch names and values. If a switch has no value an
        /// empty string is returned.
        /// </summary>
        public void GetSwitches(cef_string_map* switches)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.GetSwitches
        }
        
        /// <summary>
        /// Add a switch to the end of the command line.
        /// </summary>
        public void AppendSwitch(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.AppendSwitch
        }
        
        /// <summary>
        /// Add a switch with the specified value to the end of the command line. If
        /// the switch has no value pass an empty value string.
        /// </summary>
        public void AppendSwitchWithValue(cef_string_t* name, cef_string_t* value)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.AppendSwitchWithValue
        }
        
        /// <summary>
        /// True if there are remaining command line arguments.
        /// </summary>
        public int HasArguments()
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.HasArguments
        }
        
        /// <summary>
        /// Get the remaining command line arguments.
        /// </summary>
        public void GetArguments(cef_string_list* arguments)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.GetArguments
        }
        
        /// <summary>
        /// Add an argument to the end of the command line.
        /// </summary>
        public void AppendArgument(cef_string_t* argument)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.AppendArgument
        }
        
        /// <summary>
        /// Insert a command before the current command.
        /// Common for debuggers, like "valgrind" or "gdb --args".
        /// </summary>
        public void PrependWrapper(cef_string_t* wrapper)
        {
            throw new NotImplementedException(); // TODO: CefCommandLine.PrependWrapper
        }
        
    }
}
