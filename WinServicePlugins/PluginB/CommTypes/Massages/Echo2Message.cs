using AsyncPipeTransport.CommonTypes;
namespace Service_BPlugin.Contract.Massages
{
    public partial class MessageType
    {
        public static readonly string Echo2 = "Echo2";
    }

    public class RequestEcho2Message : MessageHeader
    {
        public string message { get; set; }

        public RequestEcho2Message(string message) : base(MessageType.Echo2) => this.message = message;
    }

    public class ResponseEcho2Message : MessageHeader
    {
        public string message { get; set; }
        public ResponseEcho2Message(string message) : base(MessageType.Echo2) => this.message = message;
    }

}