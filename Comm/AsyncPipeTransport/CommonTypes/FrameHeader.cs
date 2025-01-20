namespace AsyncPipeTransport.CommonTypes
{
    [Flags]
    public enum FrameHeaderOptions : int
    {
        None = 0x0,
        LastFrame = 0x1,
        Pulse = 0x2 | LastFrame,
        EvantMsg = 0x4 | LastFrame
    }

    public enum Opcode : int {
        OpenSession = 0
    }

    //public record struct TransportFrameHeader(long requestId, TransportFrameHeaderOptions options, Opcode msgType, string payload) { }
    public class FrameHeader
    {
        public long requestId { get; set; }
        public FrameHeaderOptions options { get; set; }
        public Opcode msgType { get; set; }
        public string payload { get; set; }
        public FrameHeader(long requestId, FrameHeaderOptions options, Opcode msgType, string payload) => 
            (this.requestId, this.options, this.msgType, this.payload) = (requestId, options, msgType, payload);
        
    }
}