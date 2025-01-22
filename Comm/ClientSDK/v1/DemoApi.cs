using CommunicationMessages.Massages;
using ServerSDK.CommonTypes;
using ServerSDK.Convertors;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;
using CommTypes.Events;
using CommunicationMessages;
using AsyncPipeTransport.Events;
using AsyncPipeTransport.CommonTypes;

namespace ClientSDK.v1
{
    public class DemoApi
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ClientChannel _client;

        //public DemoApi(ClientChannel client, ILogger<DemoApi> logger) => (_logger, _client) = (logger, client);
        public DemoApi(ClientChannel client) => (_client) = (client);

        public async Task GetAPListStream(Action<WiFiNetwork> setNextResult)
        {
            var responses = _client.RequestHandler.SendLongRequest<RespnseWiFiNetworksMessage, RequestWiFiNetworksMessage>(
                new RequestWiFiNetworksMessage());
            await foreach (var response in responses)
            {
                foreach (var network in response.list)
                {
                    if (network == null)
                        continue;

                    setNextResult(network.ToWiFiNetwork());
                }
            }
        }

        public async IAsyncEnumerable<WiFiNetwork> GetAPListAsync()
        {
            var responses = _client.RequestHandler.SendLongRequest<RespnseWiFiNetworksMessage, RequestWiFiNetworksMessage>(
                new RequestWiFiNetworksMessage());
            await foreach (var response in responses)
            {
                foreach (var network in response.list)
                {
                    if (network == null)
                        continue;
                    yield return network.ToWiFiNetwork();
                }
            }
        }

        public async Task<string?> GetEcho(string message)
        {
            var response = await _client.RequestHandler.SendRequest<ResponseEchoMessage, RequestEchoMessage>(
                new RequestEchoMessage(message));
            return response?.message;
        }

        public bool RegisterPulsEvent(Action<string> action)
        {
           return _client.EventHandler.RegisterEvent((Opcode)MessageType.PulseEvent, new EventToAction<PulseEventMessage>((pulseMsg)=>action(pulseMsg.message)));
        }
    }
}