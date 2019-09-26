using System;

namespace Xilium.CefGlue.Common.Serialization
{
    internal class CefValueHolder : CefListWrapper
    {
        private bool _isReadOnly;
        private Action<CefListValue, int> _setValue;

        public CefValueHolder() : base(null, 0)
        {
        }

        public CefValueHolder(CefListValue cefList, int index, bool isReadOnly = false) : base(cefList, index)
        {
            _isReadOnly = isReadOnly;
        }

        public override void SetNull()
        {
            _setValue = (container, index) => container.SetNull(index);
            HandleValueSet();
        }

        public override void SetBool(bool value)
        {
            _setValue = (container, index) => container.SetBool(index, value);
            HandleValueSet();
        }

        public override void SetInt(int value)
        {
            _setValue = (container, index) => container.SetInt(index, value);
            HandleValueSet();
        }

        public override void SetDouble(double value)
        {
            _setValue = (container, index) => container.SetDouble(index, value);
            HandleValueSet();
        }

        public override void SetString(string value)
        {
            _setValue = (container, index) => container.SetString(index, value);
            HandleValueSet();
        }

        public override void SetBinary(CefBinaryValue value)
        {
            _setValue = (container, index) => container.SetBinary(index, value);
            HandleValueSet();
        }

        public override void SetList(CefListValue value)
        {
            _setValue = (container, index) => container.SetList(index, value);
            HandleValueSet();
        }

        public override void SetDictionary(CefDictionaryValue value)
        {
            _setValue = (container, index) => container.SetDictionary(index, value);
            HandleValueSet();
        }

        private void HandleValueSet()
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException("This value holder is read only");
            }
            if (_container != null)
            {
                _setValue.Invoke(_container, _index);
            }
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

        public override CefBinaryValue GetBinary()
        {
            return _container.GetBinary(_index);
        }

        public override CefListValue GetList()
        {
            return _container.GetList(_index);
        }

        public override CefDictionaryValue GetDictionary()
        {
            return _container.GetDictionary(_index);
        }

        public override CefValueType GetValueType()
        {
            return _container.GetValueType(_index);
        }

        public void CopyTo(CefListValue cefList, int index)
        {
            _setValue?.Invoke(cefList, index);
        }
    }
}
