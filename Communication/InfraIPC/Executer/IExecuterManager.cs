using Intel.IntelConnect.IPC.Channel;

namespace Intel.IntelConnect.IPC.Executer
{
    public interface IExecuterManager
    {
        Task<bool> ExecuteAsync(IChannel pipeServer, string methodName, long requestId, string payload);
    }
}
