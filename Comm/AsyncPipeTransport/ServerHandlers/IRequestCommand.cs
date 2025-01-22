using AsyncPipeTransport.Channel;

namespace AsyncPipeTransport.ServerHandlers
{
    public interface IRequestCommand
    {
        Task<bool> Execute(IChannelSender sender, long requestId, string requestJson);
    }
}

