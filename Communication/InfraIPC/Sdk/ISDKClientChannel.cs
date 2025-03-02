using Intel.IntelConnect.IPC.Events;
using Intel.IntelConnect.IPC.Request;

namespace Intel.IntelConnect.IPC.Sdk.Types
{
    public interface ISDKClientChannel
    {
        event Action OnDisconnect;

        IEventManager EventHandler { get; }
        IClientRequestManager RequestHandler { get; }
    }
}
