namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a V8 value handle. V8 handles can only be accessed from
    /// the thread on which they are created. Valid threads for creating a V8 handle
    /// include the render process main thread (TID_RENDERER) and WebWorker threads.
    /// A task runner for posting tasks on the associated thread can be retrieved
    /// via the CefV8Context::GetTaskRunner() method.
    /// </summary>
    public sealed unsafe partial class CefV8Value
    {
        /// <summary>
        /// Create a new CefV8Value object of type undefined.
        /// </summary>
        public static cef_v8value_t* CreateUndefined()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateUndefined
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type null.
        /// </summary>
        public static cef_v8value_t* CreateNull()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateNull
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type bool.
        /// </summary>
        public static cef_v8value_t* CreateBool(int value)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateBool
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type int.
        /// </summary>
        public static cef_v8value_t* CreateInt(int value)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateInt
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type unsigned int.
        /// </summary>
        public static cef_v8value_t* CreateUInt(uint value)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateUInt
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type double.
        /// </summary>
        public static cef_v8value_t* CreateDouble(double value)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateDouble
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type Date. This method should only be
        /// called from within the scope of a CefRenderProcessHandler, CefV8Handler or
        /// CefV8Accessor callback, or in combination with calling Enter() and Exit()
        /// on a stored CefV8Context reference.
        /// </summary>
        public static cef_v8value_t* CreateDate(CefBaseTime date)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateDate
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type string.
        /// </summary>
        public static cef_v8value_t* CreateString(cef_string_t* value)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateString
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type object with optional accessor
        /// and/or interceptor. This method should only be called from within the
        /// scope of a CefRenderProcessHandler, CefV8Handler or CefV8Accessor
        /// callback, or in combination with calling Enter() and Exit() on a stored
        /// CefV8Context reference.
        /// </summary>
        public static cef_v8value_t* CreateObject(cef_v8accessor_t* accessor, cef_v8interceptor_t* interceptor)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateObject
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type array with the specified |length|.
        /// If |length| is negative the returned array will have length 0. This method
        /// should only be called from within the scope of a CefRenderProcessHandler,
        /// CefV8Handler or CefV8Accessor callback, or in combination with calling
        /// Enter() and Exit() on a stored CefV8Context reference.
        /// </summary>
        public static cef_v8value_t* CreateArray(int length)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateArray
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type ArrayBuffer which wraps the
        /// provided |buffer| of size |length| bytes. The ArrayBuffer is externalized,
        /// meaning that it does not own |buffer|. The caller is responsible for
        /// freeing |buffer| when requested via a call to
        /// CefV8ArrayBufferReleaseCallback::ReleaseBuffer. This method should only
        /// be called from within the scope of a CefRenderProcessHandler, CefV8Handler
        /// or CefV8Accessor callback, or in combination with calling Enter() and
        /// Exit() on a stored CefV8Context reference.
        /// NOTE: Always returns nullptr when V8 sandbox is enabled.
        /// </summary>
        public static cef_v8value_t* CreateArrayBuffer(void* buffer, UIntPtr length, cef_v8array_buffer_release_callback_t* release_callback)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateArrayBuffer
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type ArrayBuffer which copies the
        /// provided |buffer| of size |length| bytes.
        /// This method should only be called from within the scope of a
        /// CefRenderProcessHandler, CefV8Handler or CefV8Accessor callback, or in
        /// combination with calling Enter() and Exit() on a stored CefV8Context
        /// reference.
        /// </summary>
        public static cef_v8value_t* CreateArrayBufferWithCopy(void* buffer, UIntPtr length)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateArrayBufferWithCopy
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type function. This method should only
        /// be called from within the scope of a CefRenderProcessHandler, CefV8Handler
        /// or CefV8Accessor callback, or in combination with calling Enter() and
        /// Exit() on a stored CefV8Context reference.
        /// </summary>
        public static cef_v8value_t* CreateFunction(cef_string_t* name, cef_v8handler_t* handler)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreateFunction
        }
        
        /// <summary>
        /// Create a new CefV8Value object of type Promise. This method should only be
        /// called from within the scope of a CefRenderProcessHandler, CefV8Handler or
        /// CefV8Accessor callback, or in combination with calling Enter() and Exit()
        /// on a stored CefV8Context reference.
        /// </summary>
        public static cef_v8value_t* CreatePromise()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.CreatePromise
        }
        
        /// <summary>
        /// Returns true if the underlying handle is valid and it can be accessed on
        /// the current thread. Do not call any other methods if this method returns
        /// false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsValid
        }
        
        /// <summary>
        /// True if the value type is undefined.
        /// </summary>
        public int IsUndefined()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsUndefined
        }
        
        /// <summary>
        /// True if the value type is null.
        /// </summary>
        public int IsNull()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsNull
        }
        
        /// <summary>
        /// True if the value type is bool.
        /// </summary>
        public int IsBool()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsBool
        }
        
        /// <summary>
        /// True if the value type is int.
        /// </summary>
        public int IsInt()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsInt
        }
        
        /// <summary>
        /// True if the value type is unsigned int.
        /// </summary>
        public int IsUInt()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsUInt
        }
        
        /// <summary>
        /// True if the value type is double.
        /// </summary>
        public int IsDouble()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsDouble
        }
        
        /// <summary>
        /// True if the value type is Date.
        /// </summary>
        public int IsDate()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsDate
        }
        
        /// <summary>
        /// True if the value type is string.
        /// </summary>
        public int IsString()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsString
        }
        
        /// <summary>
        /// True if the value type is object.
        /// </summary>
        public int IsObject()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsObject
        }
        
        /// <summary>
        /// True if the value type is array.
        /// </summary>
        public int IsArray()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsArray
        }
        
        /// <summary>
        /// True if the value type is an ArrayBuffer.
        /// </summary>
        public int IsArrayBuffer()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsArrayBuffer
        }
        
        /// <summary>
        /// True if the value type is function.
        /// </summary>
        public int IsFunction()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsFunction
        }
        
        /// <summary>
        /// True if the value type is a Promise.
        /// </summary>
        public int IsPromise()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsPromise
        }
        
        /// <summary>
        /// Returns true if this object is pointing to the same handle as |that|
        /// object.
        /// </summary>
        public int IsSame(cef_v8value_t* that)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsSame
        }
        
        /// <summary>
        /// Return a bool value.
        /// </summary>
        public int GetBoolValue()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetBoolValue
        }
        
        /// <summary>
        /// Return an int value.
        /// </summary>
        public int GetIntValue()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetIntValue
        }
        
        /// <summary>
        /// Return an unsigned int value.
        /// </summary>
        public uint GetUIntValue()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetUIntValue
        }
        
        /// <summary>
        /// Return a double value.
        /// </summary>
        public double GetDoubleValue()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetDoubleValue
        }
        
        /// <summary>
        /// Return a Date value.
        /// </summary>
        public CefBaseTime GetDateValue()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetDateValue
        }
        
        /// <summary>
        /// Return a string value.
        /// </summary>
        public cef_string_userfree* GetStringValue()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetStringValue
        }
        
        /// <summary>
        /// Returns true if this is a user created object.
        /// </summary>
        public int IsUserCreated()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.IsUserCreated
        }
        
        /// <summary>
        /// Returns true if the last method call resulted in an exception. This
        /// attribute exists only in the scope of the current CEF value object.
        /// </summary>
        public int HasException()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.HasException
        }
        
        /// <summary>
        /// Returns the exception resulting from the last method call. This attribute
        /// exists only in the scope of the current CEF value object.
        /// </summary>
        public cef_v8exception_t* GetException()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetException
        }
        
        /// <summary>
        /// Clears the last exception and returns true on success.
        /// </summary>
        public int ClearException()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.ClearException
        }
        
        /// <summary>
        /// Returns true if this object will re-throw future exceptions. This
        /// attribute exists only in the scope of the current CEF value object.
        /// </summary>
        public int WillRethrowExceptions()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.WillRethrowExceptions
        }
        
        /// <summary>
        /// Set whether this object will re-throw future exceptions. By default
        /// exceptions are not re-thrown. If a exception is re-thrown the current
        /// context should not be accessed again until after the exception has been
        /// caught and not re-thrown. Returns true on success. This attribute exists
        /// only in the scope of the current CEF value object.
        /// </summary>
        public int SetRethrowExceptions(int rethrow)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.SetRethrowExceptions
        }
        
        /// <summary>
        /// Returns true if the object has a value with the specified identifier.
        /// </summary>
        public int HasValue(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.HasValue
        }
        
        /// <summary>
        /// Returns true if the object has a value with the specified identifier.
        /// </summary>
        public int HasValue(int index)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.HasValue
        }
        
        /// <summary>
        /// Deletes the value with the specified identifier and returns true on
        /// success. Returns false if this method is called incorrectly or an
        /// exception is thrown. For read-only and don't-delete values this method
        /// will return true even though deletion failed.
        /// </summary>
        public int DeleteValue(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.DeleteValue
        }
        
        /// <summary>
        /// Deletes the value with the specified identifier and returns true on
        /// success. Returns false if this method is called incorrectly, deletion
        /// fails or an exception is thrown. For read-only and don't-delete values
        /// this method will return true even though deletion failed.
        /// </summary>
        public int DeleteValue(int index)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.DeleteValue
        }
        
        /// <summary>
        /// Returns the value with the specified identifier on success. Returns NULL
        /// if this method is called incorrectly or an exception is thrown.
        /// </summary>
        public cef_v8value_t* GetValue(cef_string_t* key)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetValue
        }
        
        /// <summary>
        /// Returns the value with the specified identifier on success. Returns NULL
        /// if this method is called incorrectly or an exception is thrown.
        /// </summary>
        public cef_v8value_t* GetValue(int index)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetValue
        }
        
        /// <summary>
        /// Associates a value with the specified identifier and returns true on
        /// success. Returns false if this method is called incorrectly or an
        /// exception is thrown. For read-only values this method will return true
        /// even though assignment failed.
        /// </summary>
        public int SetValue(cef_string_t* key, cef_v8value_t* value, CefV8PropertyAttribute attribute)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.SetValue
        }
        
        /// <summary>
        /// Associates a value with the specified identifier and returns true on
        /// success. Returns false if this method is called incorrectly or an
        /// exception is thrown. For read-only values this method will return true
        /// even though assignment failed.
        /// </summary>
        public int SetValue(int index, cef_v8value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.SetValue
        }
        
        /// <summary>
        /// Registers an identifier and returns true on success. Access to the
        /// identifier will be forwarded to the CefV8Accessor instance passed to
        /// CefV8Value::CreateObject(). Returns false if this method is called
        /// incorrectly or an exception is thrown. For read-only values this method
        /// will return true even though assignment failed.
        /// </summary>
        public int SetValue(cef_string_t* key, CefV8PropertyAttribute attribute)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.SetValue
        }
        
        /// <summary>
        /// Read the keys for the object's values into the specified vector. Integer-
        /// based keys will also be returned as strings.
        /// </summary>
        public int GetKeys(cef_string_list* keys)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetKeys
        }
        
        /// <summary>
        /// Sets the user data for this object and returns true on success. Returns
        /// false if this method is called incorrectly. This method can only be called
        /// on user created objects.
        /// </summary>
        public int SetUserData(cef_base_ref_counted_t* user_data)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.SetUserData
        }
        
        /// <summary>
        /// Returns the user data, if any, assigned to this object.
        /// </summary>
        public cef_base_ref_counted_t* GetUserData()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetUserData
        }
        
        /// <summary>
        /// Returns the amount of externally allocated memory registered for the
        /// object.
        /// </summary>
        public int GetExternallyAllocatedMemory()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetExternallyAllocatedMemory
        }
        
        /// <summary>
        /// Adjusts the amount of registered external memory for the object. Used to
        /// give V8 an indication of the amount of externally allocated memory that is
        /// kept alive by JavaScript objects. V8 uses this information to decide when
        /// to perform global garbage collection. Each CefV8Value tracks the amount of
        /// external memory associated with it and automatically decreases the global
        /// total by the appropriate amount on its destruction. |change_in_bytes|
        /// specifies the number of bytes to adjust by. This method returns the number
        /// of bytes associated with the object after the adjustment. This method can
        /// only be called on user created objects.
        /// </summary>
        public int AdjustExternallyAllocatedMemory(int change_in_bytes)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.AdjustExternallyAllocatedMemory
        }
        
        /// <summary>
        /// Returns the number of elements in the array.
        /// </summary>
        public int GetArrayLength()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetArrayLength
        }
        
        /// <summary>
        /// Returns the ReleaseCallback object associated with the ArrayBuffer or NULL
        /// if the ArrayBuffer was not created with CreateArrayBuffer.
        /// </summary>
        public cef_v8array_buffer_release_callback_t* GetArrayBufferReleaseCallback()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetArrayBufferReleaseCallback
        }
        
        /// <summary>
        /// Prevent the ArrayBuffer from using it's memory block by setting the length
        /// to zero. This operation cannot be undone. If the ArrayBuffer was created
        /// with CreateArrayBuffer then CefV8ArrayBufferReleaseCallback::ReleaseBuffer
        /// will be called to release the underlying buffer.
        /// </summary>
        public int NeuterArrayBuffer()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.NeuterArrayBuffer
        }
        
        /// <summary>
        /// Returns the length (in bytes) of the ArrayBuffer.
        /// </summary>
        public UIntPtr GetArrayBufferByteLength()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetArrayBufferByteLength
        }
        
        /// <summary>
        /// Returns a pointer to the beginning of the memory block for this
        /// ArrayBuffer backing store. The returned pointer is valid as long as the
        /// CefV8Value is alive.
        /// </summary>
        public void* GetArrayBufferData()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetArrayBufferData
        }
        
        /// <summary>
        /// Returns the function name.
        /// </summary>
        public cef_string_userfree* GetFunctionName()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetFunctionName
        }
        
        /// <summary>
        /// Returns the function handler or NULL if not a CEF-created function.
        /// </summary>
        public cef_v8handler_t* GetFunctionHandler()
        {
            throw new NotImplementedException(); // TODO: CefV8Value.GetFunctionHandler
        }
        
        /// <summary>
        /// Execute the function using the current V8 context. This method should only
        /// be called from within the scope of a CefV8Handler or CefV8Accessor
        /// callback, or in combination with calling Enter() and Exit() on a stored
        /// CefV8Context reference. |object| is the receiver ('this' object) of the
        /// function. If |object| is empty the current context's global object will be
        /// used. |arguments| is the list of arguments that will be passed to the
        /// function. Returns the function return value on success. Returns NULL if
        /// this method is called incorrectly or an exception is thrown.
        /// </summary>
        public cef_v8value_t* ExecuteFunction(cef_v8value_t* @object, UIntPtr argumentsCount, cef_v8value_t** arguments)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.ExecuteFunction
        }
        
        /// <summary>
        /// Execute the function using the specified V8 context. |object| is the
        /// receiver ('this' object) of the function. If |object| is empty the
        /// specified context's global object will be used. |arguments| is the list of
        /// arguments that will be passed to the function. Returns the function return
        /// value on success. Returns NULL if this method is called incorrectly or an
        /// exception is thrown.
        /// </summary>
        public cef_v8value_t* ExecuteFunctionWithContext(cef_v8context_t* context, cef_v8value_t* @object, UIntPtr argumentsCount, cef_v8value_t** arguments)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.ExecuteFunctionWithContext
        }
        
        /// <summary>
        /// Resolve the Promise using the current V8 context. This method should only
        /// be called from within the scope of a CefV8Handler or CefV8Accessor
        /// callback, or in combination with calling Enter() and Exit() on a stored
        /// CefV8Context reference. |arg| is the argument passed to the resolved
        /// promise. Returns true on success. Returns false if this method is called
        /// incorrectly or an exception is thrown.
        /// </summary>
        public int ResolvePromise(cef_v8value_t* arg)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.ResolvePromise
        }
        
        /// <summary>
        /// Reject the Promise using the current V8 context. This method should only
        /// be called from within the scope of a CefV8Handler or CefV8Accessor
        /// callback, or in combination with calling Enter() and Exit() on a stored
        /// CefV8Context reference. Returns true on success. Returns false if this
        /// method is called incorrectly or an exception is thrown.
        /// </summary>
        public int RejectPromise(cef_string_t* errorMsg)
        {
            throw new NotImplementedException(); // TODO: CefV8Value.RejectPromise
        }
        
    }
}
