using AsyncPipeTransport.CommonTypes;
namespace Service_APlugin.Contract.Massages
{
    public partial class MessageType
    {
        public static readonly string Echo3 = "Echo3";
    }

    public class RequestEcho3Message : MessageHeader
    {
        public string message { get; set; }

        public RequestEcho3Message(string message) : base(MessageType.Echo3) => this.message = message;
    }

    public class ResponseEcho3Message : MessageHeader
    {
        public string message { get; set; }
        public ResponseEcho3Message(string message) : base(MessageType.Echo3) => this.message = message;
    }

}