using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.CommonTypes.InternalMassages;
using Intel.IntelConnect.IPC.Events;
using PluginA.ClientSDK.CommonTypes;
using PluginA.ClientSDK.Convertors;
using PluginA.Contract.Massages;
using Intel.IntelConnect.IPC.Sdk.Types;
namespace PluginA.ClientSDK.v1
{
    public class PluginA_Api
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ISDKClientChannel _client;
        public PluginA_Api(ISDKClientChannel client) => (_client) = (client);

        public async Task<string?> GetEcho(string message)
        {
            var response = await _client.RequestHandler.SendRequest<ResponseEchoMessage, RequestEchoMessage>(
                new RequestEchoMessage(message));
            return response?.message;
        }


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

        public async Task RegisterCpuEvent(Action<long> action)
        {
            // Start service producing events 
            var response = await _client.RequestHandler.SendRequest<NullMessage, RegisterForEventMessage>(
               new RegisterForEventMessage(MessageType.PluginA_RegisterEvent,true, [MessageType.PluginA_CpuData]));

            // listen for events on the client side
            _client.EventHandler.RegisterEvent(MessageType.PluginA_CpuData, 
                new EventToAction<GetCpuDataEventMessage>((pulseMsg) => action(pulseMsg.usage)));
        }

        public async Task UnregisterCpuEvent()
        {
            // Stop service producing events 
            var response = await _client.RequestHandler.SendRequest<NullMessage, RegisterForEventMessage>(
               new RegisterForEventMessage(MessageType.PluginA_RegisterEvent, false, [MessageType.PluginA_CpuData]));

            // stop listen for events on the client side
            _client.EventHandler.UnregisterEvent(MessageType.PluginA_CpuData);
        }
    }
}
   