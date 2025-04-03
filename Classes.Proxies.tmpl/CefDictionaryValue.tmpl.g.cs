namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a dictionary value. Can be used on any process and
    /// thread.
    /// </summary>
    public sealed unsafe partial class CefDictionaryValue
    {
        /// <summary>
        /// Creates a new object that is not owned by any other object.
        /// </summary>
        public static cef_dictionary_value_t* Create()
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.Create
        }
        
        /// <summary>
        /// Returns true if this object is valid. This object may become invalid if
        /// the underlying data is owned by another object (e.g. list or dictionary)
        /// and that other object is then modified or destroyed. Do not call any other
        /// methods if this method returns false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.IsValid
        }
        
        /// <summary>
        /// Returns true if this object is currently owned by another object.
        /// </summary>
        public int IsOwned()
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.IsOwned
        }
        
        /// <summary>
        /// Returns true if the values of this object are read-only. Some APIs may
        /// expose read-only objects.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.IsReadOnly
        }
        
        /// <summary>
        /// Returns true if this object and |that| object have the same underlying
        /// data. If true modifications to this object will also affect |that| object
        /// and vice-versa.
        /// </summary>
        public int IsSame(cef_dictionary_value_t* that)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.IsSame
        }
        
        /// <summary>
        /// Returns true if this object and |that| object have an equivalent
        /// underlying value but are not necessarily the same object.
        /// </summary>
        public int IsEqual(cef_dictionary_value_t* that)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.IsEqual
        }
        
        /// <summary>
        /// Returns a writable copy of this object. If |exclude_empty_children| is
        /// true any empty dictionaries or lists will be excluded from the copy.
        /// </summary>
        public cef_dictionary_value_t* Copy(int exclude_empty_children)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.Copy
        }
        
        /// <summary>
        /// Returns the number of values.
        /// </summary>
        public UIntPtr GetSize()
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetSize
        }
        
        /// <summary>
        /// Removes all values. Returns true on success.
        /// </summary>
        public int Clear()
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.Clear
        }
        
        /// <summary>
        /// Returns true if the current dictionary has a value for the given key.
        /// </summary>
        public int HasKey(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.HasKey
        }
        
        /// <summary>
        /// Reads all keys for this dictionary into the specified vector.
        /// </summary>
        public int GetKeys(cef_string_list* keys)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetKeys
        }
        
        /// <summary>
        /// Removes the value at the specified key. Returns true is the value was
        /// removed successfully.
        /// </summary>
        public int Remove(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.Remove
        }
        
        /// <summary>
        /// Returns the value type for the specified key.
        /// </summary>
        public CefValueType GetType(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetType
        }
        
        /// <summary>
        /// Returns the value at the specified key. For simple types the returned
        /// value will copy existing data and modifications to the value will not
        /// modify this object. For complex types (binary, dictionary and list) the
        /// returned value will reference existing data and modifications to the value
        /// will modify this object.
        /// </summary>
        public cef_value_t* GetValue(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetValue
        }
        
        /// <summary>
        /// Returns the value at the specified key as type bool.
        /// </summary>
        public int GetBool(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetBool
        }
        
        /// <summary>
        /// Returns the value at the specified key as type int.
        /// </summary>
        public int GetInt(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetInt
        }
        
        /// <summary>
        /// Returns the value at the specified key as type double.
        /// </summary>
        public double GetDouble(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetDouble
        }
        
        /// <summary>
        /// Returns the value at the specified key as type string.
        /// </summary>
        public cef_string_userfree* GetString(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetString
        }
        
        /// <summary>
        /// Returns the value at the specified key as type binary. The returned
        /// value will reference existing data.
        /// </summary>
        public cef_binary_value_t* GetBinary(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetBinary
        }
        
        /// <summary>
        /// Returns the value at the specified key as type dictionary. The returned
        /// value will reference existing data and modifications to the value will
        /// modify this object.
        /// </summary>
        public cef_dictionary_value_t* GetDictionary(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetDictionary
        }
        
        /// <summary>
        /// Returns the value at the specified key as type list. The returned value
        /// will reference existing data and modifications to the value will modify
        /// this object.
        /// </summary>
        public cef_list_value_t* GetList(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.GetList
        }
        
        /// <summary>
        /// Sets the value at the specified key. Returns true if the value was set
        /// successfully. If |value| represents simple data then the underlying data
        /// will be copied and modifications to |value| will not modify this object.
        /// If |value| represents complex data (binary, dictionary or list) then the
        /// underlying data will be referenced and modifications to |value| will
        /// modify this object.
        /// </summary>
        public int SetValue(cef_string_t* key, cef_value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetValue
        }
        
        /// <summary>
        /// Sets the value at the specified key as type null. Returns true if the
        /// value was set successfully.
        /// </summary>
        public int SetNull(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetNull
        }
        
        /// <summary>
        /// Sets the value at the specified key as type bool. Returns true if the
        /// value was set successfully.
        /// </summary>
        public int SetBool(cef_string_t* key, int value)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetBool
        }
        
        /// <summary>
        /// Sets the value at the specified key as type int. Returns true if the
        /// value was set successfully.
        /// </summary>
        public int SetInt(cef_string_t* key, int value)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetInt
        }
        
        /// <summary>
        /// Sets the value at the specified key as type double. Returns true if the
        /// value was set successfully.
        /// </summary>
        public int SetDouble(cef_string_t* key, double value)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetDouble
        }
        
        /// <summary>
        /// Sets the value at the specified key as type string. Returns true if the
        /// value was set successfully.
        /// </summary>
        public int SetString(cef_string_t* key, cef_string_t* value)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetString
        }
        
        /// <summary>
        /// Sets the value at the specified key as type binary. Returns true if the
        /// value was set successfully. If |value| is currently owned by another
        /// object then the value will be copied and the |value| reference will not
        /// change. Otherwise, ownership will be transferred to this object and the
        /// |value| reference will be invalidated.
        /// </summary>
        public int SetBinary(cef_string_t* key, cef_binary_value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetBinary
        }
        
        /// <summary>
        /// Sets the value at the specified key as type dict. Returns true if the
        /// value was set successfully. If |value| is currently owned by another
        /// object then the value will be copied and the |value| reference will not
        /// change. Otherwise, ownership will be transferred to this object and the
        /// |value| reference will be invalidated.
        /// </summary>
        public int SetDictionary(cef_string_t* key, cef_dictionary_value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetDictionary
        }
        
        /// <summary>
        /// Sets the value at the specified key as type list. Returns true if the
        /// value was set successfully. If |value| is currently owned by another
        /// object then the value will be copied and the |value| reference will not
        /// change. Otherwise, ownership will be transferred to this object and the
        /// |value| reference will be invalidated.
        /// </summary>
        public int SetList(cef_string_t* key, cef_list_value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefDictionaryValue.SetList
        }
        
    }
}
