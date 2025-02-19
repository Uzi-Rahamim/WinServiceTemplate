namespace PluginA.Contract.Types
{
    //Data structure for WiFi network item
    public class WiFiNetworkItem
    {
        // Properties
        public string ssid { get; set; }
        public int signalStrength { get; set; }
        public string securityType { get; set; }

        // Constructor
        public WiFiNetworkItem(string ssid, int signalStrength, string securityType) => (this.ssid, this.signalStrength, this.securityType) = (ssid, signalStrength, securityType);
    }
}
