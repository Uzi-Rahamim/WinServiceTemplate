using AsyncPipeTransport.Channel;

namespace AsyncPipeTransport.ServerHandlers
{
    public interface IRequestExecuter
    {
        Task<bool> Execute(IChannelSender sender, long requestId, string requestJson);
    }
}

