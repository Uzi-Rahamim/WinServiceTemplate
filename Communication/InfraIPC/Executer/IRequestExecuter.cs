using Intel.IntelConnect.IPC.Channel;

namespace Intel.IntelConnect.IPC.Executer
{
    public interface IRequestExecuter
    {
        Task<bool> ExecuteAsync(IChannelSender sender, long requestId, string requestJson);
    }
}

