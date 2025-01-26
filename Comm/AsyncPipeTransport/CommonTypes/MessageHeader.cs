namespace AsyncPipeTransport.CommonTypes
{
    //public record struct MessageHeader(Opcode msgType) { }
    public class MessageHeader
    {
        public string msgType { get; set; }

        public MessageHeader(string msgType) =>
            this.msgType = msgType;
    }
}
