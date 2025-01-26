using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace AsyncPipeTransport.ServerHandlers
{
    public abstract class BaseRequestExecuter<T, C> : IRequestExecuter where C : MessageHeader
    {
        protected ILogger<T> Log { get; private set; }
        private IChannelSender _sender = default!;
        protected long RequestId { get; private set; }

        protected abstract Task<bool> ExecuteInternal(C request);

        public static string GetSchema()
        {
            var properties = new
            {
                Type = typeof(C).Name,
                Properties = typeof(C).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new
                {
                    p.Name,
                    PropertyType = p.PropertyType.Name
                }).ToList() // Convert PropertyInfo to a simple structure
            };

            // Convert the type information (properties) to JSON
            return JsonConvert.SerializeObject(properties, Formatting.Indented);
        }

        public BaseRequestExecuter(ILogger<T> logger)
        {
            Log = logger;
            Log.LogInformation("Request Handler Created");
        }

        public Task<bool> Execute(IChannelSender sender, long requestId, string requestJson)
        {
            _sender = sender;
            RequestId = requestId;

            var requestMsg = requestJson.FromJson<C>();
            return ExecuteInternal(requestMsg);
        }

        protected Task SendLastResponse<R>(R responseMessage) where R : MessageHeader
        {
            return _sender.SendAsync(responseMessage.BuildResponseMessage(RequestId));
        }

        protected Task SendContinuingResponse<R>(R responseMessage) where R : MessageHeader
        {
            return _sender.SendAsync(responseMessage.BuildContinuingResponseMessage(RequestId));
        }

        protected Task SendEvent<R>(R eventMessage) where R : MessageHeader
        {
            return _sender.SendAsync(eventMessage.BuildServerEventMessage());
        }
    }
}