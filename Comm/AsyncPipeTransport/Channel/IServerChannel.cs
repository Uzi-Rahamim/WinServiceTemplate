namespace AsyncPipeTransport.Channel
{
    public interface IServerChannel : IDisposable, ISender
    {
        void WaitForConnection();
        new Task SendAsync(string message);
        Task<string?> ReceiveAsync();
    }
}