namespace Intel.IntelConnect.IPC.CommonTypes.InternalMassages
{
    public class NullMessage : MessageHeader
    {
        public NullMessage() : base(FrameworkMessageTypes.Empty)
        {
        }
    }
}
