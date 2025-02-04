using AsyncPipeTransport.CommonTypes;

namespace CommTypes.Massages
{
    public partial class MessageType
    {
        public static readonly string PulseEvent = "PulseEvent";
    }

    public class PulseEventMessage : MessageHeader
    {
        public string message { get; set; }

        public PulseEventMessage(string message) : base(MessageType.PulseEvent) => this.message = message;
    }
}
