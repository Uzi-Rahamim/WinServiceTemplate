using System;
using System.Collections.Generic;

namespace AsyncPipeTransport.CommonTypes
{
    [Flags]
    public enum TransportFrameHeaderOptions : int
    {
        None = 0x0,
        LastFrame = 0x1,
        ServerMsg = 0x2 | LastFrame
    }

    public enum Opcode : int { }

    //public record struct TransportFrameHeader2(long requestId, TransportFrameHeaderOptions options, Opcode msgType, string payload) { }
    //public record struct MessageHeader22(Opcode msgType) { }

   
    public class TransportFrameHeader
    {
        public long requestId { get; set; }
        public TransportFrameHeaderOptions options { get; set; }
        public Opcode msgType { get; set; }
        public string payload { get; set; }
        public TransportFrameHeader(long requestId, TransportFrameHeaderOptions options, Opcode msgType, string payload) => 
            (this.requestId, this.options, this.msgType, this.payload) = (requestId, options, msgType, payload);
        
    }

    public class MessageHeader
    {
        public Opcode msgType { get; set; }

        public MessageHeader(Opcode msgType) => 
            this.msgType = msgType;
    }
}