using Intel.IntelConnect.IPC.Channel;

namespace Intel.IntelConnect.IPC.Executer
{
    public interface IRequestExecuter
    {
        Task<bool> Execute(IChannelSender sender, long requestId, string requestJson);
    }
}

