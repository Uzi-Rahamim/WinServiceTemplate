namespace AsyncPipeTransport.Channel
{
    public interface IServerChannel : IDisposable , IChannel
    {
        Task WaitForConnectionAsync();
    }
}