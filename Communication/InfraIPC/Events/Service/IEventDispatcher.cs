using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;

namespace Intel.IntelConnect.IPC.Events.Service
{
    public interface IEventDispatcher
    {
        Task SafeRegisterForEventsAsync(Guid clientId, IChannelSender clientChannel, IEnumerable<string> topics, Func<IEnumerable<string>, Task> callback);
        Task SafeUnregisterEventsAsync(Guid clientId, IEnumerable<string> topics, Func<IEnumerable<string>, Task> callback);
        Task SafeUnregisterAllEventsAsync(Guid clientId);

        Task<bool> DispatchEventAsync<R>(R eventMessage) where R : MessageHeader;
    }
}
