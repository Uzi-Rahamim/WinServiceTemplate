namespace AsyncPipeTransport.Channel
{
    public interface IServerChannel : IDisposable, IChannelSender
    {
        void WaitForConnection();
        new Task SendAsync(string message);
        Task<string?> ReceiveAsync();
    }
}