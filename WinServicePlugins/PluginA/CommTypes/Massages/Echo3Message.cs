using Intel.IntelConnect.IPC.CommonTypes;
namespace PluginA.Contract.Massages
{
    public partial class MethodName
    {
        public const string PluginA_Echo = "PluginA.Echo";
    }

    public class RequestEchoMessage : MessageHeader
    {
        public string message { get; set; }

        public RequestEchoMessage(string message) : base(MethodName.PluginA_Echo) => this.message = message;
    }

    public class ResponseEchoMessage : MessageHeader
    {
        public string message { get; set; }
        public ResponseEchoMessage(string message) : base(MethodName.PluginA_Echo) => this.message = message;
    }
}