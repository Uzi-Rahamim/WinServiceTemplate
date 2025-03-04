using Intel.IntelConnect.IPC.CommonTypes;

namespace PluginB.Contract.Massages
{
    public partial class MethodName
    {
        public static readonly string NotifyEvant = "NotifyEvant";
    }

    public class NotifyEvantMessage : MessageHeader
    {
        public string message { get; set; }

        public NotifyEvantMessage(string message) : base(MethodName.NotifyEvant) => this.message = message;
    }

}