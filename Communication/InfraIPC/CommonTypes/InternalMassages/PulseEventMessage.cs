namespace Intel.IntelConnect.IPC.CommonTypes
{
    public class PulseEventMessage : EventMessageHeader
    {
        public PulseEventMessage() : base(FrameworkMethodName.PulseEvent) { }
    }


    public class EventRegistrationMessage : MessageHeader
    {
        public IEnumerable<string> topics { get; set; }
        public bool start { get; set; }
        public EventRegistrationMessage(bool start, IEnumerable<string> topics) => (this.topics, this.start) = (topics, start);
    }
}
