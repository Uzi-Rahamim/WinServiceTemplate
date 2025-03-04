using Intel.IntelConnect.IPC.CommonTypes;

namespace PluginA.Contract.Massages
{
    public partial class MethodName
    {   
        public const string PluginA_RegisterEvent = "PluginA.RegisterEvent";
        public const string PluginA_UnregisterEvent = "PluginA.RegisterEvent";
    }

    public partial class TopicName
    {
        public const string PluginA_CpuData = "PluginA.CpuData";
    }

    public class GetCpuDataEventMessage : EventMessageHeader
    {
        public int usage { get; set; }

        public GetCpuDataEventMessage(int usage) : base(TopicName.PluginA_CpuData) => this.usage =usage;
    }
}