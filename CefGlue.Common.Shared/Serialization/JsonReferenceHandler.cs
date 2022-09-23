using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    public class JsonReferenceHandler : ReferenceHandler
    {
        public JsonReferenceHandler() => Reset();
        private ReferenceResolver _rootedResolver;
        public override ReferenceResolver CreateResolver() => _rootedResolver;
        public void Reset() => _rootedResolver = new JsonReferencesResolver();
    }
}
