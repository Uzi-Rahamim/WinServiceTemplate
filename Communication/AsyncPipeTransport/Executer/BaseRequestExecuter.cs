using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace AsyncPipeTransport.Executer
{
    public abstract class BaseRequestExecuter<T, Rq, Rs> : IRequestExecuter where Rq : MessageHeader where Rs : MessageHeader
    {
        protected ILogger<T> Logger { get; private set; }

        protected abstract Task<Rs?> Execute(Rq request, Func<Rs, Task> sendPage);

        private readonly CancellationToken _cancellationToken;

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

        public BaseRequestExecuter(ILogger<T> logger, CancellationTokenSource cancellationToken)
        {
            Logger = logger;
            Logger.LogInformation("Request Handler Created");
            _cancellationToken = cancellationToken.Token;
        }

        public async Task<bool> Execute(IChannelSender channel, long requestId, string requestJson)
        {
            try
            {
                var requestMsg = requestJson.FromJson<Rq>();
                var response = await Execute(
                    requestMsg,
                    (responsePage) =>
                    {
                        if (channel.IsConnected())
                        {
                            return channel.SendAsync(
                            responsePage.BuildContinuingResponseMessage(requestId), CancellationToken.None);
                        }
                        Logger.LogWarning("send responsePage faile : Channel is not connected");
                        throw new TaskCanceledException();
                    });

                if (channel.IsConnected())
                {
                    if (response == null)
                        return true;
                    else
                        await channel.SendAsync(
                            response.BuildResponseMessage(requestId), CancellationToken.None);
                }
                else
                {
                    Logger.LogWarning("send response falied : Channel is not connected");
                }

                return true;
            }
            catch (TaskCanceledException)
            {
                Logger.LogWarning("Executer - Execute operation was aborted");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in Executer - Execute");
                return false;
            }
        }

    }
}