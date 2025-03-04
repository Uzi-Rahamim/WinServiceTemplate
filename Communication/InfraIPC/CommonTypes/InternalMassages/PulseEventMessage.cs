namespace Intel.IntelConnect.IPC.CommonTypes
{
    public class PulseEventMessage : MessageHeader
    {
        public PulseEventMessage() : base(FrameworkMethodName.PulseEvent) {}
    }


    public class RegisterForEventMessage : MessageHeader
    {
        public IEnumerable<string> topics { get; set; }
        public bool start { get; set; }
        public RegisterForEventMessage(string methodName,bool start, IEnumerable<string> topics) : base(methodName) => (this.topics, this.start) = (topics, start);
    }
}
