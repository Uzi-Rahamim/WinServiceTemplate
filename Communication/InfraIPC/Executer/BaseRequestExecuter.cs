using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.CommonTypes.InternalMassages;
using Intel.IntelConnect.IPC.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection;

namespace Intel.IntelConnect.IPC.Executer
{
    public abstract class BaseRequestExecuter<T, Rq, Rs> : IRequestExecuter where Rq : IMessageHeader where Rs : IMessageHeader
    {
        protected ILogger<T> Logger { get; private set; }

        protected abstract Task<Rs?> ExecuteAsync(IChannelSender channel, Rq request, Func<Rs, Task> sendNextResponse);

        protected readonly CancellationToken _cancellationToken;

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

        private async Task<Rs?> SafeExecuteAsync(IChannelSender channel, Rq request, Func<Rs, Task> sendNextResponse, Func<string, int, Task> onError)
        {
            try
            {
                return await ExecuteAsync(channel, request, sendNextResponse);
            }
            catch (Exception ex)
            {
                await onError(ex.Message,(int) ErrorCode.InternalServerError);
                throw;
            }

        }

        public async Task<bool> ExecuteAsync(IChannelSender channel, long requestId, string requestJson)
        {
            try
            {
                var requestMsg = requestJson.FromJson<Rq>();

                Logger.LogDebug("Executer --> SafeExecute");
                var response = await SafeExecuteAsync(
                    channel,
                    requestMsg,
                    (nextResponse) =>
                    {
                        if (channel.IsConnected())
                        {
                            Logger.LogDebug("Executer --> Sending Next Response Massage");
                            return channel.SendAsync(
                            nextResponse.BuildContinuingResponseMessage(requestId), _cancellationToken);
                        }
                        Logger.LogWarning("Executer --> send responsePage faile : Channel is not connected");
                        throw new TaskCanceledException();
                    },
                    (message, code) =>
                    {   
                        if (channel.IsConnected())
                        {
                            Logger.LogDebug("Executer --> Sending Error Message");
                            return channel.SendAsync(
                                    (new ErrorMessage(message, code)).BuildErrorMessage(requestId), _cancellationToken);
                        }
                        Logger.LogWarning("Executer --> send error faile : Channel is not connected");
                        throw new TaskCanceledException();
                    }
                    );

                Logger.LogDebug("Executer --> Response returned");
                if (channel.IsConnected())
                {
                    if (response == null)
                    {
                        Logger.LogDebug("Executer --> Sending NullMessage");
                        await channel.SendAsync(
                            (new NullMessage()).BuildResponseMessage(requestId), _cancellationToken);
                    }
                    else
                    {
                        Logger.LogDebug("Executer --> Sending Response");
                        await channel.SendAsync(
                            response.BuildResponseMessage(requestId), _cancellationToken);
                    }
                }
                else
                {
                    Logger.LogWarning("Executer --> send response falied : Channel is not connected");
                }

                return true;
            }
            catch (TaskCanceledException)
            {
                Logger.LogWarning("Executer --> Execute operation was aborted");
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