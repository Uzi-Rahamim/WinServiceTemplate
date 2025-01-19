using AsyncPipeTransport.Transport;
using System;
using System.Threading.Tasks;

namespace AsyncPipe.Transport
{
    public interface IServerTransport : IDisposable, ITransportSender
    {
        void WaitForConnection();
        new Task SendAsync(string message);
        Task<string?> ReceiveAsync();
    }
}