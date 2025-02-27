using AsyncPipeTransport.CommonTypes;

namespace PluginA.Contract.Massages
{
    public partial class MessageType
    {
        public static readonly string PluginA_CpuData = "PluginA.CpuData";
    }

    public class GetCpuDataEventMessage : MessageHeader
    {
        public int usage { get; set; }
        public GetCpuDataEventMessage(int usage) : base(MessageType.PluginA_CpuData) => this.usage = usage;
    }
}