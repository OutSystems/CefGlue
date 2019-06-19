using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectMethodRunnerRenderSide
    {
        //private ConcurrentDictionary<int, object> _pendingCalls;

        public NativeObjectMethodRunnerRenderSide(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.NativeObjectCallResult.Name, HandleNativeObjectCallResult);
        }
        
        private void HandleNativeObjectCallResult(MessageReceivedEventArgs args)
        {

        }
    }
}
