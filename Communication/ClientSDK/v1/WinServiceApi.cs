using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Sdk.Types;
using Intel.IntelConnect.IPC.CommonTypes.Test;


namespace Intel.IntelConnect.ClientSDK.v1
{
    public class WinServiceApi
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ISDKClientChannel _client;

        //public DemoApi(ClientChannel client, ILogger<DemoApi> logger) => (_logger, _client) = (logger, client);
        public WinServiceApi(ISDKClientChannel client) => (_client) = (client);

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
    }
}