using System;
using System.Threading.Tasks;

namespace AsyncPipe.Transport
{

    public interface IClientTransport : IDisposable
    {
        void Connect();
        Task SendAsync(string message);
        Task<string?> ReceiveAsync();

    }
}


