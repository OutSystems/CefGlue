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
        private Lazy<Dictionary<string, T>> _lazyReferenceIdToObjectMap;
        private Lazy<Dictionary<T, string>> _lazyObjectToReferenceIdMap;
        private readonly IEqualityComparer<T> _equalityComparer;

        public ReferencesResolver() : this(null) { }

        public ReferencesResolver(IEqualityComparer<T> objectComparer)
        {
            _equalityComparer = objectComparer;

            _lazyReferenceIdToObjectMap = new Lazy<Dictionary<string, T>>(() => new Dictionary<string, T>());
            _lazyObjectToReferenceIdMap = new Lazy<Dictionary<T, string>>(() => 
                _equalityComparer != null 
                ? new Dictionary<T, string>(_equalityComparer)
                : new Dictionary<T, string>());
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
            if (ReferenceIdToObjectMap.TryGetValue(referenceId, out T value))
            {
                return value;
            }

            throw new ArgumentOutOfRangeException($"Reference not found - id={referenceId}");
            
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
                return _lazyReferenceIdToObjectMap.Value;
            }
        }

        private Dictionary<T, string> ObjectToReferenceIdMap
        {
            get
            {
                return _lazyObjectToReferenceIdMap.Value;
            }
        }
    }
}
