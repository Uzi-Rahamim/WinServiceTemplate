using AsyncPipeTransport.CommonTypes;
namespace CommunicationMessages.Massages
{
    //public record RequestEchoMessage : MessageHeader(MessageType.Echo)

    public class RequestEchoMessage : MessageHeader
    {
        public string message { get; set; }

        public RequestEchoMessage(string message) : base(MessageType.Echo) => this.message = message;
    }

    public class ResponseEchoMessage : MessageHeader
    {
        public string message { get; set; }
        public ResponseEchoMessage(string message) : base(MessageType.Echo) => this.message = message;
    }

}