using Intel.IntelConnect.IPC.CommonTypes;

namespace PluginB.Contract.Massages
{
    public partial class TopicName
    {
        public static readonly string NotifyEvant = "NotifyEvant";
    }

    public class NotifyEvantMessage : EventMessageHeader
    {
        public string message { get; set; }

        public NotifyEvantMessage(string message) : base(TopicName.NotifyEvant) => this.message = message;
    }

}