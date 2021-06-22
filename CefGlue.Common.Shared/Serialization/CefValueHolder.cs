using System;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class CefValueHolder : CefValueWrapper
    {

        private CefValue _value;

        public CefValueHolder(CefValue value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public CefValueHolder()
        {
            _value = CefValue.Create();
            _value.SetNull();
        }

        public override void SetNull()
        {
            _value.SetNull();
        }

        public override void SetBool(bool value)
        {
            _value.SetBool(value);
        }

        public override void SetInt(int value)
        {
            _value.SetInt(value);
        }

        public override void SetDouble(double value)
        {
            _value.SetDouble(value);
        }

        public override void SetString(string value)
        {
            _value.SetString(value);
        }

        public override void SetBinary(ICefBinaryValue value)
        {
            _value.SetBinary(value as CefBinaryValue);
        }

        public override void SetList(ICefListValue value)
        {
            _value.SetList(value as CefListValue);
        }

        public override void SetDictionary(ICefDictionaryValue value)
        {
            _value.SetDictionary(value as CefDictionaryValue);
        }

        public override bool GetBool()
        {
            return _value.GetBool();
        }

        public override int GetInt()
        {
            return _value.GetInt();
        }

        public override double GetDouble()
        {
            return _value.GetDouble();
        }

        public override string GetString()
        {
            return _value.GetString();
        }

        public override ICefBinaryValue GetBinary()
        {
            return _value.GetBinary();
        }

        public override ICefListValue GetList()
        {
            return _value.GetList();
        }

        public override ICefDictionaryValue GetDictionary()
        {
            return _value.GetDictionary();
        }

        public override CefValueType GetValueType()
        {
            return _value.GetValueType();
        }

        /// <summary>
        /// Utility method to assign the value held by this holder into a list entry,
        /// and then immediately discarding the value. This is done because if the value
        /// being assigned is a complex value (binary/list/dictionary), its ownership
        /// will change and our reference will become invalid. Later attempts to use
        /// that reference, for example when the GC is running finalizers and calling
        /// <c>release()</c> on the objects can crash or have nasty side effects because
        /// we'll be using memory that no longer belongs to the structure.
        /// </summary>
        public void AssignToListAndClearReference(ICefListValue list, int position)
        {
            list.SetValue(position, _value);
            _value.SetNull();
            _value.Dispose();
            _value = null;
        }
    }

}
