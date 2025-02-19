using AsyncPipeTransport.CommonTypes;
namespace PluginB.Contract.Massages
{
    public partial class MessageType
    {
        public static readonly string PluginB_Echo = "PluginB.Echo";
    }

    public class RequestEchoMessage : MessageHeader
    {
        public string message { get; set; }

        public RequestEchoMessage(string message) : base(MessageType.PluginB_Echo) => this.message = message;
    }

    public class ResponseEchoMessage : MessageHeader
    {
        public string message { get; set; }
        public ResponseEchoMessage(string message) : base(MessageType.PluginB_Echo) => this.message = message;
    }

}