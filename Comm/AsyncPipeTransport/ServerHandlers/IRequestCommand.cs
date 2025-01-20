using AsyncPipeTransport.Channel;

namespace AsyncPipeTransport.ServerHandlers
{
    public interface IRequestCommand
    {
        Task<bool> Execute(ISender sender, long requestId, string requestJson);
    }
}

