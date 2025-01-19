using AsyncPipeTransport.CommonTypes;
namespace CommunicationMessages.Massages
{

    //public record RequestEchoMessage : MessageHeader((Opcode)MessageType.Echo)

    public class RequestEchoMessage : MessageHeader
    {
        public string message { get; set; }

        public RequestEchoMessage(string message) : base((Opcode)MessageType.Echo) => this.message = message;
    }
    public class ResponseEchoMessage : MessageHeader
    {
        public string message { get; set; }
        public ResponseEchoMessage(string message) : base((Opcode)MessageType.Echo) => this.message = message;
    }

}