using System.Threading;

namespace Intel.IntelConnect.IPC.Channel
{
    public interface IServerChannel : IDisposable , IChannel
    {
        Task WaitForConnectionAsync(CancellationToken cancellationToken);
    }
}