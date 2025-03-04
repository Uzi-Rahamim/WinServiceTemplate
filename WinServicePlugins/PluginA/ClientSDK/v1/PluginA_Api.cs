using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.CommonTypes.InternalMassages;
using PluginA.ClientSDK.CommonTypes;
using PluginA.ClientSDK.Convertors;
using PluginA.Contract.Massages;
using Intel.IntelConnect.IPC.Sdk.Types;
using Intel.IntelConnect.IPC.Events.Client;
namespace PluginA.ClientSDK.v1
{
    public class PluginA_Api
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ISDKClientChannel _client;
        public PluginA_Api(ISDKClientChannel client) => (_client) = (client);

        public async Task<string?> GetEchoAsync(string message)
        {
            var response = await _client.RequestHandler.SendRequestAsync<ResponseEchoMessage, RequestEchoMessage>(
                MethodName.PluginA_Echo,
                new RequestEchoMessage(message));
            return response?.message;
        }


#if NET8_0_OR_GREATER
        public async IAsyncEnumerable<WiFiNetwork> GetAPListAsync()
        {
            var responses = _client.RequestHandler.SendLongRequestAsync<RespnseWiFiNetworksMessage, RequestWiFiNetworksMessage>(
                MethodName.APList,
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
        public async Task GetAPListStreamAsync(Action<WiFiNetwork> setNextResult)
        {
            var responses = _client.RequestHandler.SendLongRequestAsync<RespnseWiFiNetworksMessage, RequestWiFiNetworksMessage>(
                MethodName.APList,
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

        public async Task RegisterCpuEventAsync(Action<long> action)
        {
            // Start service producing events 
            var response = await _client.RequestHandler.SendRequestAsync<NullMessage, RegisterForEventMessage>(
               MethodName.PluginA_RegisterEvent,
               new RegisterForEventMessage(true, [TopicName.PluginA_CpuData]));

            // listen for events on the client side
            _client.EventHandler.RegisterEvent(TopicName.PluginA_CpuData, 
                new EventToAction<GetCpuDataEventMessage>((pulseMsg) => action(pulseMsg.usage)));
        }

        public async Task UnregisterCpuEventAsync()
        {
            // Stop service producing events 
            var response = await _client.RequestHandler.SendRequestAsync<NullMessage, RegisterForEventMessage>(
                MethodName.PluginA_UnregisterEvent,
               new RegisterForEventMessage(false, [TopicName.PluginA_CpuData]));

            // stop listen for events on the client side
            _client.EventHandler.UnregisterEvent(TopicName.PluginA_CpuData);
        }
    }
}
   