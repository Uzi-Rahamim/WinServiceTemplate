using Intel.IntelConnect.IPC.CommonTypes;

namespace PluginA.Contract.Massages
{
    public partial class MethodName
    {   
        public const string PluginA_EventRegistration = "PluginA.EventRegistration";
    }

    public partial class TopicName
    {
        public const string PluginA_CpuData = "PluginA.CpuDataEvent";
    }

    public class CpuDataEventMessage : EventMessageHeader
    {
        public int usage { get; set; }
        public CpuDataEventMessage(int usage) : base(TopicName.PluginA_CpuData) => this.usage =usage;
    }
}