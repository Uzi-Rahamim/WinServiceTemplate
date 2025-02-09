using AsyncPipeTransport.Events;
using ClientSDK.v1;
using Service_ExecuterPlugin.CommTypes.Massages;
namespace Service_ExecuterPlugin.ClientSDK.v1
{
    public class Api
    {
        ///private readonly ILogger<DemoApi> _logger;
        private readonly ClientChannel _client;
        public Api(ClientChannel client) => (_client) = (client);

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