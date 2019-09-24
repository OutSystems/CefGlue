namespace Xilium.CefGlue.Common.Serialization
{
    internal class CefListWrapper : CefValueWrapper<int, CefListValue>
    {
        public CefListWrapper(CefListValue container, int index) : base(container, index)
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

        public override void SetBinary(CefBinaryValue value)
        {
            _container.SetBinary(_index, value);
        }

        public override void SetList(CefListValue value)
        {
            _container.SetList(_index, value);
        }

        public override void SetDictionary(CefDictionaryValue value)
        {
            _container.SetDictionary(_index, value);
        }
    }
}
