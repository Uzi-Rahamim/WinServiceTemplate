using AsyncPipeTransport.Channel;

namespace AsyncPipeTransport.ServerHandlers
{
    public interface IServerRequestsManager
    {
        public Task HandleClient(IServerChannel pipeServer, long clientId);
    }
}
