using AsyncPipeTransport.Channel;
using Microsoft.Extensions.Logging;
using AsyncPipeTransport.Extensions;
using Service_BPlugin.Contract.Massages;

namespace Service_BPlugin.Worker
{
    public class SimpleWorker
    {
        ILogger<SimpleWorker> _logger;
        public string Message { get=> "SimpleWorker B"; }
        public IChannelSender? _channel { get; private set; }
        public SimpleWorker(ILogger<SimpleWorker> logger)
        {
            _logger = logger;
            _logger.LogInformation("SimpleWorker B created");
        }

        public void SetChannel(IChannelSender? channel)
        {
            if (channel == null)
            {
                _logger.LogError("Channel is null");
                return;
            }
            _channel = channel;
            _= Task.Run(async () => {
                _logger.LogInformation("Start worker Notify ");
                try
                {
                    while (channel.IsConnected())
                    {
                        _logger.LogInformation("SendNotify ");
                        await _channel.SendAsync(new NotifyEvantMessage("Notify To Client").BuildServerEventMessage(), CancellationToken.None);
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Error in SimpleWorkerB");
                }
                
            });
        }

        public void Start()
        {
            _logger.LogInformation("SimpleWorkerB started");
        }
    }
}
