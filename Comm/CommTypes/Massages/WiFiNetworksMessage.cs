using AsyncPipeTransport.CommonTypes;
using System.Collections.Generic;

namespace CommunicationMessages.Massages
{

    public class RequestWiFiNetworksMessage : MessageHeader
    {
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