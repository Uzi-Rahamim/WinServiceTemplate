using Intel.IntelConnect.IPC.Request;
using Intel.IntelConnect.IPC.Events.Client;

namespace Intel.IntelConnect.IPC.Sdk.Types
{
    public interface ISDKClientChannel
    {
        event Action OnDisconnect;

        IEventManager EventHandler { get; }
        IClientRequestManager RequestHandler { get; }
    }
}
