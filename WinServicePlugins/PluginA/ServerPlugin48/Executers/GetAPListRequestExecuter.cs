using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using PluginA.Contract.Massages;
using PluginA.Contract.Types;

namespace PluginA.Executers
{
    public class GetAPListRequestExecuter : StreamResponseRequestExecuter<GetAPListRequestExecuter, RequestWiFiNetworksMessage, RespnseWiFiNetworksMessage>
    {
        public GetAPListRequestExecuter(ILogger<GetAPListRequestExecuter> logger, CancellationTokenSource cts) : base(logger, cts) { }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return MessageType.APList;
        }

        protected override async Task<RespnseWiFiNetworksMessage?> Execute(
            RequestWiFiNetworksMessage requestMsg, 
            Func<RespnseWiFiNetworksMessage, Task> sendPage)
        {

            for (int page = 0; page < 100; page++)
            {
                await Task.Delay(1000);
                Logger.LogInformation($"Server sent page {page}");
                await sendPage(new RespnseWiFiNetworksMessage(wifiNetworks));
            }

            Logger.LogInformation("Server sent last page");
            return new RespnseWiFiNetworksMessage(wifiNetworks);
        }

        protected override async IAsyncEnumerable<RespnseWiFiNetworksMessage> Execute(RequestWiFiNetworksMessage request)
        {
            for (int page = 0; page < 2; page++)
            {
                await Task.Delay(1000);
                Logger.LogInformation($"Server sent page {page}");
                yield return new RespnseWiFiNetworksMessage(wifiNetworks);
            }

            Logger.LogInformation("Server End Sending");
            yield break;
        }


        // Create a list of 100 mock WiFi networks
        private readonly List<WiFiNetworkItem> wifiNetworks = new List<WiFiNetworkItem>
            {
                new WiFiNetworkItem ("Network_1", -65,"WPA2"),
                new WiFiNetworkItem ("Network_2", -70,"WPA3"),
                new WiFiNetworkItem ("Network_3", -80,"WPA2"),
                new WiFiNetworkItem ("Network_4", -55,"WEP"),
                new WiFiNetworkItem ("Network_5", -60,"None"),
                new WiFiNetworkItem ("Network_6", -72,"WPA2"),
                new WiFiNetworkItem ("Network_7", -65,"WPA3"),
                new WiFiNetworkItem ("Network_8", -50,"WPA2"),
                new WiFiNetworkItem ("Network_9", -75,"WEP"),
                new WiFiNetworkItem ("Network_10", -85,"None"),
                new WiFiNetworkItem ("Network_11", -68,"WPA2"),
                new WiFiNetworkItem ("Network_12", -78,"WPA3"),
                new WiFiNetworkItem ("Network_13", -66,"WPA2"),
                new WiFiNetworkItem ("Network_14", -90,"None"),
                new WiFiNetworkItem ("Network_15", -72,"WPA2"),
                new WiFiNetworkItem ("Network_16", -85,"WPA3"),
                new WiFiNetworkItem ("Network_17", -60,"WPA2"),
                new WiFiNetworkItem ("Network_18", -77,"WEP"),
                new WiFiNetworkItem ("Network_19", -55,"None"),
                new WiFiNetworkItem ("Network_20", -80,"WPA2"),
            };
    }
}
