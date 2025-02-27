using AsyncPipeTransport.CommonTypes;
namespace PluginA.Contract.Massages
{
    public partial class MessageType
    {
        public static readonly string PluginA_Echo = "PluginA.Echo";
    }

    public class RequestEchoMessage : MessageHeader
    {
        public string message { get; set; }

        public RequestEchoMessage(string message) : base(MessageType.PluginA_Echo) => this.message = message;
    }

    public class ResponseEchoMessage : MessageHeader
    {
        public string message { get; set; }
        public ResponseEchoMessage(string message) : base(MessageType.PluginA_Echo) => this.message = message;
    }
}