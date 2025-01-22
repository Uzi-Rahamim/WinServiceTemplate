using System.Threading.Tasks;


namespace AsyncPipeTransport.Channel
{
    public interface IChannelSender
    {
        public Task SendAsync(string message);
    }
}
