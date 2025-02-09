using AsyncPipeTransport.Channel;
using AsyncPipeTransport.Clients;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Events;
using AsyncPipeTransport.Executer;
using AsyncPipeTransport.Extensions;
using AsyncPipeTransport.Request;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.Listeners
{

    public class MessageListener : IDisposable
    {
        protected readonly IChannel _channel;
        private readonly IEventManager? _clientEventHandler;
        private readonly IClientRequestManager? _clientRequestHandler;
        private readonly IExecuterManager? _executerManager;
        private readonly IClientsManager? _activeClients;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private bool _disposed = false;
        public event Action? OnDisconnect;


        public MessageListener(
            ILogger logger,
            CancellationToken cancellationToken,
            IChannel channel,
            IClientRequestManager? clientRequestHandler,
            IEventManager? clientEventHandler,
            IExecuterManager? executerManager,
            IClientsManager? activeClients)
        {
            _logger = logger;
            _cancellationToken = cancellationToken;
            _channel = channel;
            _clientRequestHandler = clientRequestHandler;
            _clientEventHandler = clientEventHandler;
            _executerManager = executerManager;
            _activeClients = activeClients;
        }

        private void OnDisconnectInternal()
        {
            var disconnectEvent = OnDisconnect;
            disconnectEvent?.Invoke();
        }

        public void StartListen(TimeSpan timeout, long endpointId)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogDebug("MessageListener Start");
                    await StartReadMessageLoop(timeout, endpointId);
                    _logger.LogDebug("MessageListener Terminate");
                }
                catch (Exception ex) when (
                ex is TimeoutException ||
                ex is OperationCanceledException ||
                ex is IOException)
                {
                    _logger.LogDebug("MessageListener Terminate - {type}", ex.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in MessageListener");
                }
                finally
                {
                    OnDisconnectInternal();
                }
            });

            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogDebug("PulseEvantGenerator Start");
                    await StartPulseEvantGenerator();
                    _logger.LogDebug("PulseEvantGenerator Terminate");
                }
                catch (Exception ex) when (
                ex is TimeoutException || 
                ex is OperationCanceledException ||
                ex is IOException)
                {
                    _logger.LogDebug("PulseEvantGenerator Terminate - {type} ", ex.GetType().Name);
                }
                catch (Exception ex)
                {   
                    if (_disposed)
                        _logger.LogDebug("PulseEvantGenerator Terminate - {type} ", ex.GetType().Name);
                    else
                        _logger.LogError(ex, "Error in PulseEvantGenerator");
                }
            });
        }

        private async Task StartReadMessageLoop(TimeSpan timeout, long endpointId)
        {
            bool channelIsSecure = false;
            while (!_disposed)
            {
                string? messageStr = await _channel.ReceiveAsync(_cancellationToken);
                if (messageStr == null)
                    break;

                var frame = messageStr.ExtractFrameHeaders();
                if (frame == null)
                {
                    _logger.LogInformation($"Receive an invalid message");
                    continue;
                }
                else if (frame.IsEventFrame()) 
                {
                    if (_clientEventHandler != null && _executerManager==null) //Ignore event on server side
                        _clientEventHandler.HandleEvent(frame);
                }
                else if (frame.IsResponseFrame() && _clientRequestHandler != null && _clientRequestHandler.GetPendingRequest(frame.requestId, out ClientRequest? request))
                {
                    request?.PushResponse(frame);
                }
                else if (frame.IsRequestFrame() && _executerManager != null) 
                {
                    if (!channelIsSecure)
                    {
                        if (frame.IsOpenSessionFrame())
                        {
                            channelIsSecure = await _executerManager.Execute(_channel, FrameworkMessageTypes.OpenSession, frame.requestId, frame.payload, endpointId);
                            if (!channelIsSecure)
                                break;

                            //_activeClients.AddClient(clientId, _channel);
                        }
                        _logger.LogInformation("Channel is not secure {frame.requestId} ignore request", frame.requestId);
                        continue;
                    }
                    _ = Task.Run(async () =>
                    {
                        await _executerManager.Execute(_channel, frame.msgType, frame.requestId, frame.payload, endpointId);
                    });
                    _logger.LogInformation("No executer {frame.requestId} found for this response", frame.requestId);
                }
                else
                {
                    _logger.LogInformation("Unknow message {frame.msgType} ", frame.msgType);
                }
            }
            //_activeClients.RemoveClient(clientId);
        }

        private async Task StartPulseEvantGenerator()
        {
            await Task.Delay(Consts.MaxConnectionMonitorInterval);
            while (!_disposed)
            {   
                await _channel.SendAsync(new PulseEventMessage().BuildServerEventMessage(), _cancellationToken);
                await Task.Delay(Consts.MaxConnectionMonitorInterval);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _logger.LogDebug("Disposed");
            _disposed = true;
            _channel.Dispose();
        }
    }

}
