namespace Intel.IntelConnect.IPC.CommonTypes
{
    //public record struct TransportFrameHeader(long requestId, TransportFrameHeaderOptions options, Opcode methodName, string payload) { }
    public class FrameHeader
    {
        public long requestId { get; set; }
        public FrameOptions options { get; set; }
        public string methodName { get; set; }
        public string payload { get; set; }
        public FrameHeader(long requestId, FrameOptions options, string methodName, string payload) => 
            (this.requestId, this.options, this.methodName, this.payload) = (requestId, options, methodName, payload);
    }
}