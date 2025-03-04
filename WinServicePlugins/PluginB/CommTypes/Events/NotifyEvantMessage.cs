using Intel.IntelConnect.IPC.CommonTypes;

namespace PluginB.Contract.Massages
{
    public partial class TopicName
    {
        public static readonly string NotifyEvant = "NotifyEvant";
    }

    public class NotifyEvantMessage : IEventMessageHeader
    {
        public string message { get; set; }

        public string topic => TopicName.NotifyEvant;

        public NotifyEvantMessage(string message)  => this.message = message;
    }

}