using Intel.IntelConnect.IPC.Attributes;
using Intel.IntelConnect.IPC.v1.Executer;
using Microsoft.Extensions.Logging;
using PluginA.Contract.Massages;
using PluginA.Contract.Types;

namespace PluginA.Executers
{
    [Executer<RequestWiFiNetworksMessage, RespnseWiFiNetworksMessage>(MethodName.APList)]
    public class GetAPListRequestExecuter : StreamRequestExecuter<GetAPListRequestExecuter, RequestWiFiNetworksMessage, RespnseWiFiNetworksMessage>
    {
        public GetAPListRequestExecuter(ILogger<GetAPListRequestExecuter> logger, CancellationTokenSource cts) : base(logger, cts) { }


        protected override async IAsyncEnumerable<RespnseWiFiNetworksMessage> ExecuteAsync(RequestWiFiNetworksMessage request)
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
