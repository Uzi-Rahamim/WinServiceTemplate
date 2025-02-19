using Service_APlugin.Contract.Massages;
using WinServicePluginCommon.Sdk.Types;
namespace Service_APlugin.ClientSDK.v1
{
    public class Api
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ISDKClientChannel _client;
        public Api(ISDKClientChannel client) => (_client) = (client);

        public async Task<string?> GetEcho(string message)
        {
            var response = await _client.RequestHandler.SendRequest<ResponseEcho3Message, RequestEcho3Message>(
                new RequestEcho3Message(message));
            return response?.message;
        }
    }
}