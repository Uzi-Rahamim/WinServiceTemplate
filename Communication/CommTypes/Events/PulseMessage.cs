using AsyncPipeTransport.CommonTypes;

namespace CommTypes.Massages
{
    public partial class MessageType
    {
        public static readonly string BPulseEvent = "PulseEvent";
    }

    public class BPulseEventMessage : MessageHeader
    {
        public string message { get; set; }

        public BPulseEventMessage(string message) : base(MessageType.BPulseEvent) => this.message = message;
    }
}
