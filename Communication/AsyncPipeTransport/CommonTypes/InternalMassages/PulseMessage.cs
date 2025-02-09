namespace AsyncPipeTransport.CommonTypes
{
    public partial class FrameworkMessageTypes
    {
        public static readonly string PulseEvent = "___PulseEvent";
    }

    public class PulseEventMessage : MessageHeader
    {
        public PulseEventMessage() : base(FrameworkMessageTypes.PulseEvent) {}
    }
}
