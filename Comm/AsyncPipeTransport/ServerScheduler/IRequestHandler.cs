
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Transport;
using System.Threading.Tasks;


namespace AsyncPipeTransport.RequestHandler
{
    public interface IRequestHandler
    {
        Task Execute(ITransportSender sender, long requestId, string requestJson);
    }
}

