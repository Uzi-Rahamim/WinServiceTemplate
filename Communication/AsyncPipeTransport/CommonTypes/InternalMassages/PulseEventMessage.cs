namespace AsyncPipeTransport.CommonTypes
{
    public class PulseEventMessage : MessageHeader
    {
        public PulseEventMessage() : base(FrameworkMessageTypes.PulseEvent) {}
    }


    public class RegisterForEventMessage : MessageHeader
    {
        public IEnumerable<string> topics { get; set; }
        public RegisterForEventMessage(IEnumerable<string> topics) : base(FrameworkMessageTypes.RegisterEvent) => this.topics = topics;
    }
}
