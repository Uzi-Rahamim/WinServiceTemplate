using Intel.IntelConnect.IPC.Channel;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.Listeners
{
    public interface IClientMessageListener : IDisposable
    {
        Task<bool> StartAsync(CancellationToken cancellationToken, TimeSpan timeout, long endpointId = 0);
    }

    public class ClientMessageListener : IClientMessageListener
    {
        private readonly MessageListener _messageListener;
        private readonly IClientChannel _channel;
        private readonly ILogger<ClientMessageListener> _logger;


        public ClientMessageListener(
            ILogger<ClientMessageListener> logger,
            IClientChannel channel,
            MessageListener messageListener)
        {
            _logger = logger;
            _channel = channel;
            _messageListener = messageListener;
        }

        public void Dispose()
        {
            _messageListener?.Dispose();
        }

        public async Task<bool> StartAsync(CancellationToken cancellationToken, TimeSpan timeout, long endpointId = 0)
        {
            _logger.LogInformation("StartAsync");
            var channel = _channel;
            try
            {
                await channel.ConnectAsync(timeout, cancellationToken);
            }
            catch (TimeoutException)
            {
                return false;
            }
            
            _messageListener.StartListen(timeout, endpointId);
            return true;
        }

    }
}
