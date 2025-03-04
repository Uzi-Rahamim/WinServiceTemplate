namespace Intel.IntelConnect.IPC.CommonTypes
{
    public interface IMessageHeader
    {
    }

    public interface IEventMessageHeader : IMessageHeader
    {
        string topic { get; }
    }
}
