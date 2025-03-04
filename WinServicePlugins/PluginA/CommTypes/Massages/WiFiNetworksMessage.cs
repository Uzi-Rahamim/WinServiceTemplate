using Intel.IntelConnect.IPC.CommonTypes;
using PluginA.Contract.Types;

namespace PluginA.Contract.Massages
{
    public partial class MethodName
    {
        public const string APList = "PluginA.APList";
    }

    public class RequestWiFiNetworksMessage : IMessageHeader
    {
        public int interval { get; set; }
    }

    public class RespnseWiFiNetworksMessage : IMessageHeader
    {
        public IEnumerable<WiFiNetworkItem> list { get; set; }
        public RespnseWiFiNetworksMessage(IEnumerable<WiFiNetworkItem> list) => this.list = list;
    }

    
}