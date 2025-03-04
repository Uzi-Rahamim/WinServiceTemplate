using Intel.IntelConnect.IPC.CommonTypes;
using PluginA.Contract.Types;

namespace PluginA.Contract.Massages
{
    public partial class MethodName
    {
        public const string APList = "PluginA.APList";
    }

    public class RequestWiFiNetworksMessage : MessageHeader
    {
        public int interval { get; set; }
        public RequestWiFiNetworksMessage() : base(MethodName.APList) { }
    }

    public class RespnseWiFiNetworksMessage : MessageHeader
    {
        public IEnumerable<WiFiNetworkItem> list { get; set; }
        public RespnseWiFiNetworksMessage(IEnumerable<WiFiNetworkItem> list) : base(MethodName.APList) => this.list = list;
    }

    
}