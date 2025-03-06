namespace Intel.IntelConnect.IPC.CommonTypes
{
    
    public abstract class MessageHeader
    {
        //optional informative only
        public string? messageName { get; set; } = null;

        protected MessageHeader() {
#if DEBUG
            // Set the derived class name using reflection (only for debug)
            messageName = this.GetType().FullName?? this.GetType().Name;
#endif
        }
    }

    public abstract class EventMessageHeader : MessageHeader
    {
        public string topic { get; }

        protected EventMessageHeader(string topic) => this.topic = topic;
    }
}
