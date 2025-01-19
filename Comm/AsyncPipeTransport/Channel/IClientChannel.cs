namespace AsyncPipeTransport.Channel
{
    public interface IClientChannel : IDisposable
    {
        void Connect();
        Task SendAsync(string message);
        Task<string?> ReceiveAsync();
    }
}


