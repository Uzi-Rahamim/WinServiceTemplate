using AsyncPipeTransport.CommonTypes;
namespace CommTypes.Massages
{
    //public record RequestEchoMessage : MessageHeader(MessageType.Echo)
    public partial class MessageType
    {
        public static readonly string Echo = "Echo";
    }

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