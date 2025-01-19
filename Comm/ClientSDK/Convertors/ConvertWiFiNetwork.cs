using ServerSDK.CommonTypes;
using System;

namespace ServerSDK.Convertors 
{
    public static class ConvertWiFiNetwork
    {
        public static WiFiNetwork ToWiFiNetwork(this CommunicationMessages.Massages.WiFiNetworkItem obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            return new WiFiNetwork(obj.ssid, obj.signalStrength, obj.securityType);
        }
    }
}