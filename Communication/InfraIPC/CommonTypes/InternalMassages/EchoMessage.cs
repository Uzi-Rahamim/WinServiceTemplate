namespace Intel.IntelConnect.IPC.CommonTypes.Test
{
    public class RequestEchoMessage : IMessageHeader
    {
        public string message { get; set; }

        public RequestEchoMessage(string message)  => this.message = message;
    }

    public class ResponseEchoMessage : IMessageHeader
    {
        public string message { get; set; }
        public ResponseEchoMessage(string message) => this.message = message;
    }

}