
using PluginA.ClientSDK.CommonTypes;

namespace PluginA.ClientSDK.Convertors 
{
    public static class ConvertWiFiNetwork
    {
        public static WiFiNetwork ToWiFiNetwork(this PluginA.Contract.Types.WiFiNetworkItem obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            return new WiFiNetwork(obj.ssid, obj.signalStrength, obj.securityType);
        }
    }
}