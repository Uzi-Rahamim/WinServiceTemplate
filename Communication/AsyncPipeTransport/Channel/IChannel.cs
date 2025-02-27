namespace AsyncPipeTransport.Channel
{
    public interface IChannel: IChannelSender, IDisposable
    {
        
        event Action OnDisconnect;
        Task<string?> ReceiveAsync(CancellationToken cancellationToken);
    }
}
