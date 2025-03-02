namespace Intel.IntelConnect.IPC.CommonTypes
{
    public class PulseEventMessage : MessageHeader
    {
        public PulseEventMessage() : base(FrameworkMessageTypes.PulseEvent) {}
    }


    public class RegisterForEventMessage : MessageHeader
    {
        public IEnumerable<string> topics { get; set; }
        public bool start { get; set; }
        public RegisterForEventMessage(string msgType,bool start, IEnumerable<string> topics) : base(msgType) => (this.topics, this.start) = (topics, start);
    }
}
