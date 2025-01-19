namespace AsyncPipeTransport.CommonTypes
{
    //public record struct MessageHeader(Opcode msgType) { }
    public class MessageHeader
    {
        public Opcode msgType { get; set; }

        public MessageHeader(Opcode msgType) =>
            this.msgType = msgType;
    }
}
