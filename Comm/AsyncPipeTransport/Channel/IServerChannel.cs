using System.Threading;

namespace AsyncPipeTransport.Channel
{
    public interface IServerChannel : IDisposable , IChannel
    {
        Task WaitForConnectionAsync(CancellationToken cancellationToken);
    }
}