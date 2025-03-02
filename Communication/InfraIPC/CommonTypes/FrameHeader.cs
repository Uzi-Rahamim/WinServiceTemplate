namespace Intel.IntelConnect.IPC.CommonTypes
{
    //public record struct TransportFrameHeader(long requestId, TransportFrameHeaderOptions options, Opcode msgType, string payload) { }
    public class FrameHeader
    {
        public long requestId { get; set; }
        public FrameOptions options { get; set; }
        public string msgType { get; set; }
        public string payload { get; set; }
        public FrameHeader(long requestId, FrameOptions options, string msgType, string payload) => 
            (this.requestId, this.options, this.msgType, this.payload) = (requestId, options, msgType, payload);
    }
}