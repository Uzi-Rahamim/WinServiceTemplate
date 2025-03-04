using Intel.IntelConnect.IPC.Sdk.Types;
using Intel.IntelConnect.IPC.Events.Client;
using PluginB.Contract.Massages;

namespace PluginB.ClientSDK.v1
{
    public class PluginB_Api
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ISDKClientChannel _client;
        public PluginB_Api(ISDKClientChannel client) => (_client) = (client);

        public async Task<string?> GetEchoAsync(string message)
        {
            var response = await _client.RequestHandler.SendRequestAsync<ResponseEchoMessage, RequestEchoMessage>(
                new RequestEchoMessage(message));
            return response?.message;
        }

        public bool RegisterNotifyEvent(Action<string> action)
        {
            return _client.EventHandler.RegisterEvent(MethodName.NotifyEvant, new EventToAction<NotifyEvantMessage>((pulseMsg) => action(pulseMsg.message)));
        }
    }
}