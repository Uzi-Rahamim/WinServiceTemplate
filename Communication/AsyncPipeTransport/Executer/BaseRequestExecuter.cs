using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace AsyncPipeTransport.Executer
{
    public abstract class BaseRequestExecuter<T, Rq, Rs> : IRequestExecuter where Rq : MessageHeader
    {
        protected ILogger<T> Log { get; private set; }
        protected IChannelSender Channel { get; private set; }
        protected long RequestId { get; private set; }

        protected abstract Task<bool> Execute(Rq request);

        public static string GetSchema()
        {
            var properties = new
            {
                request = new
                {
                    name = typeof(Rq).Name,
                    properties = typeof(Rq).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new
                    {
                        name = p.Name,
                        type = p.PropertyType.Name
                    }).ToList() // Convert PropertyInfo to a simple structure
                },
                response = new
                {
                    name = typeof(Rs).Name,
                    properties = typeof(Rs).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new
                    {
                        name = p.Name,
                        type = p.PropertyType.Name
                    }).ToList() // Convert PropertyInfo to a simple structure
                }
            };

            // Convert the type information (properties) to JSON
            return JsonConvert.SerializeObject(properties, Formatting.Indented);
        }

        public BaseRequestExecuter(ILogger<T> logger, CancellationTokenSource cts)
        {
            Log = logger;
            Log.LogInformation("Request Handler Created");
        }

        public Task<bool> Execute(IChannelSender channel, long requestId, string requestJson)
        {
            Channel = channel;
            RequestId = requestId;

            try
            {
                var requestMsg = requestJson.FromJson<Rq>();
                return Execute(requestMsg);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error in Execute");
                return Task.FromResult(false);
            }
        }

        protected Task SendLastResponse<R>(R responseMessage) where R : MessageHeader
        {
            return Channel.SendAsync(responseMessage.BuildResponseMessage(RequestId), CancellationToken.None);
        }

        protected Task SendContinuingResponse<R>(R responseMessage) where R : MessageHeader
        {
            return Channel.SendAsync(responseMessage.BuildContinuingResponseMessage(RequestId), CancellationToken.None);
        }

        protected Task SendEvent<R>(R eventMessage) where R : MessageHeader
        {
            return Channel.SendAsync(eventMessage.BuildServerEventMessage(), CancellationToken.None);
        }
    }
}