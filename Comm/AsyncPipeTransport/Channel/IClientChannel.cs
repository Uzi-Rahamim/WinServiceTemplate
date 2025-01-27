namespace AsyncPipeTransport.Channel
{
    public interface IClientChannel : IDisposable
    {
        Task ConnectAsync(TimeSpan timeout);
        Task SendAsync(string message);
        Task<string?> ReceiveAsync();
    }
}


