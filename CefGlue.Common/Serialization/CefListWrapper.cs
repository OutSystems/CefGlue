namespace Xilium.CefGlue.Common.Serialization
{
    internal class CefListWrapper : CefValueWrapper<int, ICefListValue>
    {
        public CefListWrapper(ICefListValue container, int index) : base(container, index)
        {
        }

        public override void SetNull()
        {
            _container.SetNull(_index);
        }

        public override void SetBool(bool value)
        {
            _container.SetBool(_index, value);
        }

        public override void SetInt(int value)
        {
            _container.SetInt(_index, value);
        }

        public override void SetDouble(double value)
        {
            _container.SetDouble(_index, value);
        }

        public override void SetString(string value)
        {
            _container.SetString(_index, value);
        }

        public override void SetBinary(ICefBinaryValue value)
        {
            _container.SetBinary(_index, value as CefBinaryValue);
        }

        public override void SetList(ICefListValue value)
        {
            _container.SetList(_index, value as CefListValue);
        }

        public override void SetDictionary(ICefDictionaryValue value)
        {
            _container.SetDictionary(_index, value as CefDictionaryValue);
        }

        public override bool GetBool()
        {
            return _container.GetBool(_index);
        }

        public override int GetInt()
        {
            return _container.GetInt(_index);
        }

        public override double GetDouble()
        {
            return _container.GetDouble(_index);
        }

        public override string GetString()
        {
            return _container.GetString(_index);
        }

        public override ICefBinaryValue GetBinary()
        {
            return _container.GetBinary(_index);
        }

        public override ICefListValue GetList()
        {
            return _container.GetList(_index);
        }

        public override ICefDictionaryValue GetDictionary()
        {
            return _container.GetDictionary(_index);
        }

        public override CefValueType GetValueType()
        {
            return _container.GetValueType(_index);
        }

        public int Count => _container.Count;
    }
}
