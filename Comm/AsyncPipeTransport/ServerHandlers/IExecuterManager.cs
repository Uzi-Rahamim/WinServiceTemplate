using AsyncPipeTransport.Channel;

namespace AsyncPipeTransport.ServerHandlers
{
    public interface IExecuterManager
    {
        Task<bool> Execute(IChannel pipeServer, string msgType, long requestId, string payload, long clientId);
    }
}
