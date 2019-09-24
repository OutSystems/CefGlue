namespace Xilium.CefGlue.Common.Serialization
{
    internal abstract class CefValueWrapper
    {
        public abstract void SetNull();
        public abstract void SetBool(bool value);
        public abstract void SetInt(int value);
        public abstract void SetDouble(double value);
        public abstract void SetString(string value);
        public abstract void SetBinary(CefBinaryValue value);
        public abstract void SetList(CefListValue value);
        public abstract void SetDictionary(CefDictionaryValue value);
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
