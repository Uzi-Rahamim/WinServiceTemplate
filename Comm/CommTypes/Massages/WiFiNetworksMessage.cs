using AsyncPipeTransport.CommonTypes;

namespace CommTypes.Massages
{
    public partial class MessageType
    {
        public static readonly string APList = "APList";
    }

    public class RequestWiFiNetworksMessage : MessageHeader
    {
        public int interval { get; set; }
        public RequestWiFiNetworksMessage() : base(MessageType.APList) { }
    }

    public class WiFiNetworkItem
    {
        // Properties
        public string ssid { get; set; }
        public int signalStrength { get; set; }
        public string securityType { get; set; }

        // Constructor
        public WiFiNetworkItem(string ssid, int signalStrength, string securityType) => (this.ssid, this.signalStrength, this.securityType) = (ssid, signalStrength, securityType);
    }

    public class RespnseWiFiNetworksMessage : MessageHeader
    {
        public IEnumerable<WiFiNetworkItem> list { get; set; }
        public RespnseWiFiNetworksMessage(IEnumerable<WiFiNetworkItem> list) : base(MessageType.APList) => this.list = list;
    }
}