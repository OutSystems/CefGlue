using System;
using System.Collections.Generic;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ReferencesResolver<T> : IReferencesResolver<T>
    {
        private Lazy<Dictionary<string, T>> _lazyReferenceIdToObjectMap;
        
        public ReferencesResolver()
        {
            _lazyReferenceIdToObjectMap = new Lazy<Dictionary<string, T>>(() => new Dictionary<string, T>());
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
        }

        public T ResolveReference(string referenceId)
        {
            if (ReferenceIdToObjectMap.TryGetValue(referenceId, out T value))
            {
                return value;
            }

            throw new ArgumentOutOfRangeException($"Reference not found - id={referenceId}");
            
        }

        private Dictionary<string, T> ReferenceIdToObjectMap
        {
            get
            {
                return _lazyReferenceIdToObjectMap.Value;
            }
        }
    }
}
