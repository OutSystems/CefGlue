namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that wraps other data value types. Complex types (binary, dictionary
    /// and list) will be referenced but not owned by this object. Can be used on
    /// any process and thread.
    /// </summary>
    public sealed unsafe partial class CefValue
    {
        /// <summary>
        /// Creates a new object.
        /// </summary>
        public static cef_value_t* Create()
        {
            throw new NotImplementedException(); // TODO: CefValue.Create
        }
        
        /// <summary>
        /// Returns true if the underlying data is valid. This will always be true for
        /// simple types. For complex types (binary, dictionary and list) the
        /// underlying data may become invalid if owned by another object (e.g. list
        /// or dictionary) and that other object is then modified or destroyed. This
        /// value object can be re-used by calling Set*() even if the underlying data
        /// is invalid.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefValue.IsValid
        }
        
        /// <summary>
        /// Returns true if the underlying data is owned by another object.
        /// </summary>
        public int IsOwned()
        {
            throw new NotImplementedException(); // TODO: CefValue.IsOwned
        }
        
        /// <summary>
        /// Returns true if the underlying data is read-only. Some APIs may expose
        /// read-only objects.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefValue.IsReadOnly
        }
        
        /// <summary>
        /// Returns true if this object and |that| object have the same underlying
        /// data. If true modifications to this object will also affect |that| object
        /// and vice-versa.
        /// </summary>
        public int IsSame(cef_value_t* that)
        {
            throw new NotImplementedException(); // TODO: CefValue.IsSame
        }
        
        /// <summary>
        /// Returns true if this object and |that| object have an equivalent
        /// underlying value but are not necessarily the same object.
        /// </summary>
        public int IsEqual(cef_value_t* that)
        {
            throw new NotImplementedException(); // TODO: CefValue.IsEqual
        }
        
        /// <summary>
        /// Returns a copy of this object. The underlying data will also be copied.
        /// </summary>
        public cef_value_t* Copy()
        {
            throw new NotImplementedException(); // TODO: CefValue.Copy
        }
        
        /// <summary>
        /// Returns the underlying value type.
        /// </summary>
        public CefValueType GetType()
        {
            throw new NotImplementedException(); // TODO: CefValue.GetType
        }
        
        /// <summary>
        /// Returns the underlying value as type bool.
        /// </summary>
        public int GetBool()
        {
            throw new NotImplementedException(); // TODO: CefValue.GetBool
        }
        
        /// <summary>
        /// Returns the underlying value as type int.
        /// </summary>
        public int GetInt()
        {
            throw new NotImplementedException(); // TODO: CefValue.GetInt
        }
        
        /// <summary>
        /// Returns the underlying value as type double.
        /// </summary>
        public double GetDouble()
        {
            throw new NotImplementedException(); // TODO: CefValue.GetDouble
        }
        
        /// <summary>
        /// Returns the underlying value as type string.
        /// </summary>
        public cef_string_userfree* GetString()
        {
            throw new NotImplementedException(); // TODO: CefValue.GetString
        }
        
        /// <summary>
        /// Returns the underlying value as type binary. The returned reference may
        /// become invalid if the value is owned by another object or if ownership is
        /// transferred to another object in the future. To maintain a reference to
        /// the value after assigning ownership to a dictionary or list pass this
        /// object to the SetValue() method instead of passing the returned reference
        /// to SetBinary().
        /// </summary>
        public cef_binary_value_t* GetBinary()
        {
            throw new NotImplementedException(); // TODO: CefValue.GetBinary
        }
        
        /// <summary>
        /// Returns the underlying value as type dictionary. The returned reference
        /// may become invalid if the value is owned by another object or if ownership
        /// is transferred to another object in the future. To maintain a reference to
        /// the value after assigning ownership to a dictionary or list pass this
        /// object to the SetValue() method instead of passing the returned reference
        /// to SetDictionary().
        /// </summary>
        public cef_dictionary_value_t* GetDictionary()
        {
            throw new NotImplementedException(); // TODO: CefValue.GetDictionary
        }
        
        /// <summary>
        /// Returns the underlying value as type list. The returned reference may
        /// become invalid if the value is owned by another object or if ownership is
        /// transferred to another object in the future. To maintain a reference to
        /// the value after assigning ownership to a dictionary or list pass this
        /// object to the SetValue() method instead of passing the returned reference
        /// to SetList().
        /// </summary>
        public cef_list_value_t* GetList()
        {
            throw new NotImplementedException(); // TODO: CefValue.GetList
        }
        
        /// <summary>
        /// Sets the underlying value as type null. Returns true if the value was set
        /// successfully.
        /// </summary>
        public int SetNull()
        {
            throw new NotImplementedException(); // TODO: CefValue.SetNull
        }
        
        /// <summary>
        /// Sets the underlying value as type bool. Returns true if the value was set
        /// successfully.
        /// </summary>
        public int SetBool(int value)
        {
            throw new NotImplementedException(); // TODO: CefValue.SetBool
        }
        
        /// <summary>
        /// Sets the underlying value as type int. Returns true if the value was set
        /// successfully.
        /// </summary>
        public int SetInt(int value)
        {
            throw new NotImplementedException(); // TODO: CefValue.SetInt
        }
        
        /// <summary>
        /// Sets the underlying value as type double. Returns true if the value was
        /// set successfully.
        /// </summary>
        public int SetDouble(double value)
        {
            throw new NotImplementedException(); // TODO: CefValue.SetDouble
        }
        
        /// <summary>
        /// Sets the underlying value as type string. Returns true if the value was
        /// set successfully.
        /// </summary>
        public int SetString(cef_string_t* value)
        {
            throw new NotImplementedException(); // TODO: CefValue.SetString
        }
        
        /// <summary>
        /// Sets the underlying value as type binary. Returns true if the value was
        /// set successfully. This object keeps a reference to |value| and ownership
        /// of the underlying data remains unchanged.
        /// </summary>
        public int SetBinary(cef_binary_value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefValue.SetBinary
        }
        
        /// <summary>
        /// Sets the underlying value as type dict. Returns true if the value was set
        /// successfully. This object keeps a reference to |value| and ownership of
        /// the underlying data remains unchanged.
        /// </summary>
        public int SetDictionary(cef_dictionary_value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefValue.SetDictionary
        }
        
        /// <summary>
        /// Sets the underlying value as type list. Returns true if the value was set
        /// successfully. This object keeps a reference to |value| and ownership of
        /// the underlying data remains unchanged.
        /// </summary>
        public int SetList(cef_list_value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefValue.SetList
        }
        
    }
}
