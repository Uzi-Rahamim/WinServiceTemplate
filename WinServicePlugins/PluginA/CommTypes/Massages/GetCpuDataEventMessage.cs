using Intel.IntelConnect.IPC.CommonTypes;

namespace PluginA.Contract.Massages
{
    public partial class MessageType
    {
        public static readonly string PluginA_CpuData = "PluginA.CpuData";
        public static readonly string PluginA_RegisterEvent = "PluginA_RegisterEvent";
    }

    public class GetCpuDataEventMessage : MessageHeader
    {
        public int usage { get; set; }
        public GetCpuDataEventMessage(int usage) : base(MessageType.PluginA_CpuData) => this.usage = usage;
    }
}