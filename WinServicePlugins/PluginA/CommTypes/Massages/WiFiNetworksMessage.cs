using AsyncPipeTransport.CommonTypes;
using PluginA.Contract.Types;

namespace PluginA.Contract.Massages
{
    public partial class MessageType
    {
        public static readonly string APList = "PluginA.APList";
    }

    public class RequestWiFiNetworksMessage : MessageHeader
    {
        public int interval { get; set; }
        public RequestWiFiNetworksMessage() : base(MessageType.APList) { }
    }

    public class RespnseWiFiNetworksMessage : MessageHeader
    {
        public IEnumerable<WiFiNetworkItem> list { get; set; }
        public RespnseWiFiNetworksMessage(IEnumerable<WiFiNetworkItem> list) : base(MessageType.APList) => this.list = list;
    }

    
}