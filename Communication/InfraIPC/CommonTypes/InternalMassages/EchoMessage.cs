namespace Intel.IntelConnect.IPC.CommonTypes.Test
{
    public class RequestEchoMessage : MessageHeader
    {
        public string message { get; set; }

        public RequestEchoMessage(string message) : base(FrameworkMethodName.Echo) => this.message = message;
    }

    public class ResponseEchoMessage : MessageHeader
    {
        public string message { get; set; }
        public ResponseEchoMessage(string message) : base(FrameworkMethodName.Echo) => this.message = message;
    }

}