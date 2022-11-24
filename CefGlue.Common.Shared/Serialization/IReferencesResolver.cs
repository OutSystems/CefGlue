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
        string AddReference(T value);

        void AddReference(string referenceId, T value);

        T ResolveReference(string referenceId);

        bool TryGetReferenceId(T value, out string referenceId);
    }
}
