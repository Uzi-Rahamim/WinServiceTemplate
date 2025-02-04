namespace AsyncPipeTransport.Channel
{
    public interface IChannel: IChannelSender, IDisposable
    {
        Task<string?> ReceiveAsync(CancellationToken cancellationToken);
    }
}
