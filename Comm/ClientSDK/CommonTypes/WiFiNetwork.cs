using AsyncPipeTransport.CommonTypes;
using CommunicationMessages;

namespace ServerSDK.CommonTypes
{
    //public class WiFiNetwork(string ssid, int signalStrength, string securityType) { 
    //}
    public class WiFiNetwork
    {
        // Properties
        public string ssid { get; set; }
        public int signalStrength { get; set; }
        public string securityType { get; set; }

        // Constructor
        public WiFiNetwork(string ssid, int signalStrength, string securityType) => (this.ssid, this.signalStrength, this.securityType) = (ssid, signalStrength, securityType);
    }
}