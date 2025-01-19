using System.Threading.Tasks;

namespace AsyncPipeTransport.Transport
{
    public interface ITransportSender
    {
        public Task SendAsync(string message);
    }
}
