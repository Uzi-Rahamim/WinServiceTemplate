using PluginA.ClientSDK.CommonTypes;
using PluginA.ClientSDK.Convertors;
using PluginA.Contract.Massages;
using WinServicePluginCommon.Sdk.Types;
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
    }
}