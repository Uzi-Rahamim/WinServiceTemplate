using AsyncPipeTransport.Events;
using AsyncPipeTransport.Request;

namespace WinServicePluginCommon.Sdk.Types
{
    public interface ISDKClientChannel
    {
        event Action OnDisconnect;

        IEventManager EventHandler { get; }
        IClientRequestManager RequestHandler { get; }
    }
}
