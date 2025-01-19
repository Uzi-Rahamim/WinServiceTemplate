using AsyncPipeTransport.Channel;

namespace AsyncPipeTransport.ServerScheduler
{
    public interface IRequestHandler
    {
        Task Execute(ISender sender, long requestId, string requestJson);
    }
}

