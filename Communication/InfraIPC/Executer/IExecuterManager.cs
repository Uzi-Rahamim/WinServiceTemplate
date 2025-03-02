using Intel.IntelConnect.IPC.Channel;

namespace Intel.IntelConnect.IPC.Executer
{
    public interface IExecuterManager
    {
        Task<bool> Execute(IChannel pipeServer, string msgType, long requestId, string payload, long clientId);
    }
}
