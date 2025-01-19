using CommunicationMessages.Massages;
using AsyncPipeTransport.ServerScheduler;
using CommTypes.Events;

namespace App.WindowsService.API.Requests
{
    public class GetAPListRequestHandler : BaseRequestHandler<GetAPListRequestHandler, RequestWiFiNetworksMessage>
    {

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

        public GetAPListRequestHandler(ILogger<GetAPListRequestHandler> logger) : base(logger) { }

        protected override async Task ExecuteInternal(RequestWiFiNetworksMessage requestMsg)
        {
            Task.Delay(1000).Wait();
            Log.LogInformation("Server sent page 1" );
            await SendContinuingResponse(new RespnseWiFiNetworksMessage(wifiNetworks));
            await SendEvent(new PulseEventMessage("PulseEvent"));
            Task.Delay(1000).Wait();
            Log.LogInformation("Server sent page 2");
            await SendContinuingResponse(new RespnseWiFiNetworksMessage(wifiNetworks));
            await SendEvent(new PulseEventMessage("PulseEvent"));
            Task.Delay(1000).Wait();
            Log.LogInformation("Server sent page 3");
            await SendLastResponse(new RespnseWiFiNetworksMessage(wifiNetworks));
            await SendEvent(new PulseEventMessage("PulseEvent"));

            // Send a response back to the client
            string replyPalyload = $"Request # {RequestId} ";
            Log.LogInformation("Server sent reply: {reply}", replyPalyload);
        }
    }
}
