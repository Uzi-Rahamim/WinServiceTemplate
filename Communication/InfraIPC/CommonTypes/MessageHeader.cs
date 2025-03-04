namespace Intel.IntelConnect.IPC.CommonTypes
{
    
    public abstract class MessageHeader
    {
        //informative only
        public string messageName { get; set; }

        protected MessageHeader() {

            // Set the derived class name using reflection
            messageName = this.GetType().FullName?? this.GetType().Name;
        }
    }

    public abstract class EventMessageHeader : MessageHeader
    {
        public string topic { get; }

        protected EventMessageHeader(string topic) => this.topic = topic;

    }
}
