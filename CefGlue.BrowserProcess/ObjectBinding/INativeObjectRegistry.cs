using System.Threading.Tasks;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal interface INativeObjectRegistry
    {
        Task<bool> Bind(string objName);
        void Unbind(string objName);
    }
}
