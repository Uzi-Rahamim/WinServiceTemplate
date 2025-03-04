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

    public class GetCpuDataEventMessage : IEventMessageHeader
    {
        public int usage { get; set; }
        public string topic { get => TopicName.PluginA_CpuData; }

        public GetCpuDataEventMessage(int usage) => this.usage =usage;
    }
}