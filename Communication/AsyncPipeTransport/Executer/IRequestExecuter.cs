using AsyncPipeTransport.Channel;

namespace AsyncPipeTransport.Executer
{
    public interface IRequestExecuter
    {
        Task<bool> Execute(IChannelSender sender, long requestId, string requestJson);
    }
}

