using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ReferencesResolver<T> : IReferencesResolver<T>
    {
        private Dictionary<string, T> _referenceIdToObjectMap;
        private Dictionary<T, string> _objectToReferenceIdMap;
        private readonly IEqualityComparer<T> _equalityComparer;

        public ReferencesResolver() { }

        public ReferencesResolver(IEqualityComparer<T> objectComparer) : base()
        {
            _equalityComparer = objectComparer;
        }

        public string AddReference(T value)
        {
            var refId = ReferenceIdToObjectMap.Count.ToString();
            AddReference(refId, value);
            return refId;
        }

        public void AddReference(string referenceId, T value)
        {
            ReferenceIdToObjectMap.Add(referenceId, value);
            ObjectToReferenceIdMap.Add(value, referenceId);
        }

        public T ResolveReference(string referenceId)
        {
            if (!ReferenceIdToObjectMap.TryGetValue(referenceId, out T value))
            {
                throw new ArgumentException($"Reference not found - id={referenceId}");
            }

            return value;
        }

        public bool TryGetReferenceId(T value, out string referenceId)
        {
            if (_equalityComparer != null)
            {
                var key = ObjectToReferenceIdMap.Keys.FirstOrDefault(k => _equalityComparer.Equals(k, value));
                var foundReference = key != null;
                referenceId = foundReference ? ObjectToReferenceIdMap[key] : null;
                return foundReference;
            }
            return ObjectToReferenceIdMap.TryGetValue(value, out referenceId);
        }

        private Dictionary<string, T> ReferenceIdToObjectMap
        {
            get
            {
                if (_referenceIdToObjectMap == null)
                {
                    _referenceIdToObjectMap = new Dictionary<string, T>();
                    _objectToReferenceIdMap = _equalityComparer != null
                        ? new Dictionary<T, string>(_equalityComparer)
                        : new Dictionary<T, string>();
                }
                return _referenceIdToObjectMap;
            }
        }

        private Dictionary<T, string> ObjectToReferenceIdMap
        {
            get
            {
                if (_objectToReferenceIdMap == null)
                {
                    _objectToReferenceIdMap = _equalityComparer != null
                        ? new Dictionary<T, string>(_equalityComparer)
                        : new Dictionary<T, string>();
                }
                return _objectToReferenceIdMap;
            }
        }
    }
}
