using AsyncPipeTransport.Channel;

namespace AsyncPipeTransport.ServerHandlers
{
    public interface IServerRequestHandler
    {
        public Task HandleClient(IServerChannel pipeServer, long clientId);
    }
}
