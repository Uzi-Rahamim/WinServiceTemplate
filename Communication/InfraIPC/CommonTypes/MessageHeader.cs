namespace Intel.IntelConnect.IPC.CommonTypes
{
    //public record struct MessageHeader(Opcode methodName) { }
    public class MessageHeader
    {
        public string methodName { get; set; }

        public MessageHeader(string methodName) =>
            this.methodName = methodName;
    }
}
