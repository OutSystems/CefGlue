using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    public class JsonReferencesResolver : ReferenceResolver
    {
        private readonly Dictionary<string, object> _referenceIdToObjectMap;

        public JsonReferencesResolver() => _referenceIdToObjectMap = new Dictionary<string, object>();

        public override void AddReference(string referenceId, object value)
        {
            Debug.Assert(_referenceIdToObjectMap != null);

            if (_referenceIdToObjectMap.ContainsKey(referenceId))
            {
                throw new JsonException($"Duplicate reference id found - id={referenceId}");
            }

            _referenceIdToObjectMap.Add(referenceId, value);
        }

        public override string GetReference(object value, out bool alreadyExists)
        {
            throw new NotSupportedException("GetReference is not supported in JsonReferencesResolver");
        }

        public override object ResolveReference(string referenceId)
        {
            Debug.Assert(_referenceIdToObjectMap != null);

            if (!_referenceIdToObjectMap.TryGetValue(referenceId, out object value))
            {
                throw new JsonException($"Reference not found - id={referenceId}");
            }

            return value;
        }
    }
}