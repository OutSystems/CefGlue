namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal abstract class CefValueWrapper
    {
        public abstract void SetNull();
        public abstract void SetBool(bool value);
        public abstract void SetInt(int value);
        public abstract void SetDouble(double value);
        public abstract void SetString(string value);
        public abstract void SetBinary(ICefBinaryValue value);
        public abstract void SetList(ICefListValue value);
        public abstract void SetDictionary(ICefDictionaryValue value);

        public abstract bool GetBool();
        public abstract int GetInt();
        public abstract double GetDouble();
        public abstract string GetString();
        public abstract ICefBinaryValue GetBinary();
        public abstract ICefListValue GetList();
        public abstract ICefDictionaryValue GetDictionary();

        public abstract CefValueType GetValueType();
    }

    internal abstract class CefValueWrapper<TIndex, TCefContainerUnderlyingType> : CefValueWrapper
    {
        protected readonly TIndex _index;
        protected readonly TCefContainerUnderlyingType _container;

        public CefValueWrapper(TCefContainerUnderlyingType container, TIndex index)
        {
            _index = index;
            _container = container;
        }
    }
}
