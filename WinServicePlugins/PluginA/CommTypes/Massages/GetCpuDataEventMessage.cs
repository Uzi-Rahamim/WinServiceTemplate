using Intel.IntelConnect.IPC.CommonTypes;

namespace PluginA.Contract.Massages
{
    public partial class TopicName
    {
        public const string PluginA_CpuData = "PluginA.CpuData";
    }
    public partial class MethodName
    {   
        public const string PluginA_RegisterEvent = "PluginA.RegisterEvent";
    }

    public class GetCpuDataEventMessage : MessageHeader
    {
        public int usage { get; set; }
        public GetCpuDataEventMessage(int usage) : base(TopicName.PluginA_CpuData) => this.usage = usage;
    }
}