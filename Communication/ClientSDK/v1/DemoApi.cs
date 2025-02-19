using ServerSDK.CommonTypes;
using ServerSDK.Convertors;
using AsyncPipeTransport.Events;
using AsyncPipeTransport.CommonTypes;
using CommTypes.Massages;
using WinServicePluginCommon.Sdk.Types;


namespace ClientSDK.v1
{
    public class DemoApi
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ISDKClientChannel _client;

        //public DemoApi(ClientChannel client, ILogger<DemoApi> logger) => (_logger, _client) = (logger, client);
        public DemoApi(ISDKClientChannel client) => (_client) = (client);


#if NET8_0_OR_GREATER
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
 #elif NETFRAMEWORK
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
#endif
       

        public async IAsyncEnumerable<string> GetSchema()
        {
            var responses = _client.RequestHandler.SendLongRequest<ResponseSchemaMessage, RequestSchemaMessage>(
                new RequestSchemaMessage());
            await foreach (var response in responses)
            {
                if (response == null)
                    continue;
                yield return response.schema;
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
           return _client.EventHandler.RegisterEvent(MessageType.BPulseEvent, new EventToAction<BPulseEventMessage>((pulseMsg)=>action(pulseMsg.message)));
        }
    }
}