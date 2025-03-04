namespace Intel.IntelConnect.IPC.CommonTypes
{
    public class PulseEventMessage : IEventMessageHeader
    {
        public string topic => FrameworkMethodName.PulseEvent;
    }


    public class RegisterForEventMessage : IMessageHeader
    {
        public IEnumerable<string> topics { get; set; }
        public bool start { get; set; }
        public RegisterForEventMessage(bool start, IEnumerable<string> topics) => (this.topics, this.start) = (topics, start);
    }
}
