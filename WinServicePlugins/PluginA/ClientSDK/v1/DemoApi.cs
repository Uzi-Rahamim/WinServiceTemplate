using ClientSDK.v1;
using Service_48_ExecuterPlugin.CommTypes.Massages;
using System.Collections.Generic;
namespace Service_48_ExecuterPlugin.ClientSDK.v1
{
    public class Api
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ClientChannel _client;
        public Api(ClientChannel client) => (_client) = (client);

        public async Task<string?> GetEcho(string message)
        {
            var response = await _client.RequestHandler.SendRequest<ResponseEcho3Message, RequestEcho3Message>(
                new RequestEcho3Message(message));
            return response?.message;
        }

        public async Task<IEnumerable<string>> GetPlatformCompatibility()
        {
            var response = await _client.RequestHandler.SendRequest<ResponseGetPlatformCompatibilityMessage, RequestGetPlatformCompatibilityMessage>(
                new RequestGetPlatformCompatibilityMessage());
            return response?.list ?? new List<string>();
        }
    }
}