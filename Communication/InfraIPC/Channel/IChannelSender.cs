using System.Threading.Tasks;


namespace Intel.IntelConnect.IPC.Channel
{
    public interface IChannelSender
    {
        Guid ChannelId { get; }
        public Task SendAsync(string message, CancellationToken cancellationToken);
        bool IsConnected();
    }
}
