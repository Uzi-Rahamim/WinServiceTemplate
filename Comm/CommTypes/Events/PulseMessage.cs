using AsyncPipeTransport.CommonTypes;
using CommunicationMessages;

namespace CommTypes.Events
{
    public class PulseEventMessage : MessageHeader
    {
        public string message { get; set; }

        public PulseEventMessage(string message) : base((Opcode)MessageType.PulseEvent) => this.message = message;
    }
}
