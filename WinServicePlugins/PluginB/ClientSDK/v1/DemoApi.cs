using AsyncPipeTransport.Events;
using Service_BPlugin.Contract.Massages;
using WinServicePluginCommon.Sdk.Types;
namespace Service_BPlugin.ClientSDK.v1
{
    public class Api
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ISDKClientChannel _client;
        public Api(ISDKClientChannel client) => (_client) = (client);

        public async Task<string?> GetEcho(string message)
        {
            var response = await _client.RequestHandler.SendRequest<ResponseEcho2Message, RequestEcho2Message>(
                new RequestEcho2Message(message));
            return response?.message;
        }

        public bool RegisterNotifyEvent(Action<string> action)
        {
            return _client.EventHandler.RegisterEvent(MessageType.NotifyEvant, new EventToAction<NotifyEvantMessage>((pulseMsg) => action(pulseMsg.message)));
        }
    }
}