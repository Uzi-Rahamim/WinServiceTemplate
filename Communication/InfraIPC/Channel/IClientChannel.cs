namespace Intel.IntelConnect.IPC.Channel
{
    public interface IClientChannel : IChannel, IDisposable
    {
        Task ConnectAsync(TimeSpan timeout, CancellationToken cancellationToken);
    }
}


