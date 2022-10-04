using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal interface IReferencesResolver<T>
    {
        public int ReferencesCount { get; }

        public void AddReference(string referenceId, T value);

        public T ResolveReference(string referenceId);

        public bool TryGetReferenceId(T value, out string referenceId);
    }
}
