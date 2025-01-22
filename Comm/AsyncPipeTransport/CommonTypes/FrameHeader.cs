namespace AsyncPipeTransport.CommonTypes
{
    [Flags]
    public enum FrameOptions : int
    {
        None = 0x00,
        LastFrame = 0x01,
        Security = 0x02,
        
        Request = 0x10,
        Response = 0x20,
        Pulse = 0x40 | LastFrame,
        EvantMsg = 0x80 | LastFrame
    }

    public enum Opcode : int {
        OpenSession = 0
    }

    //public record struct TransportFrameHeader(long requestId, TransportFrameHeaderOptions options, Opcode msgType, string payload) { }
    public class FrameHeader
    {
        public long requestId { get; set; }
        public FrameOptions options { get; set; }
        public Opcode msgType { get; set; }
        public string payload { get; set; }
        public FrameHeader(long requestId, FrameOptions options, Opcode msgType, string payload) => 
            (this.requestId, this.options, this.msgType, this.payload) = (requestId, options, msgType, payload);
        
    }
}