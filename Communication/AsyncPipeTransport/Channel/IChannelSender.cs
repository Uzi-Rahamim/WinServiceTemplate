using System.Threading.Tasks;


namespace AsyncPipeTransport.Channel
{
    public interface IChannelSender
    {
        Guid ChannelId { get; }
        public Task SendAsync(string message, CancellationToken cancellationToken);
        bool IsConnected();
    }
}
