using AsyncPipeTransport.Channel;
using Microsoft.Extensions.Logging;
using AsyncPipeTransport.Extensions;
using Service_BPlugin.Contract.Massages;

namespace Service_APlugin.Worker
{
    public class SimpleWorkerB
    {
        ILogger<SimpleWorkerB> _logger;
        public string Message { get=> "SimpleWorkerB"; }
        public IChannelSender? _channel { get; private set; }
        public SimpleWorkerB(ILogger<SimpleWorkerB> logger)
        {
            _logger = logger;
            _logger.LogInformation("SimpleWorkerB created");
        }


        public void SetChannel(IChannelSender channel)
        {
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
