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
        private readonly IEventDispatcher? _eventDispatcher;
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
            IEventDispatcher? eventDispatcher)
        {
            _logger = logger;
            _cancellationToken = cancellationToken;
            _channel = channel;
            _clientRequestHandler = clientRequestHandler;
            _clientEventHandler = clientEventHandler;
            _executerManager = executerManager;
            _eventDispatcher = eventDispatcher;
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
                            {
                                _logger.LogWarning("Channel open session failed, close the channel");
                                break;
                            }
                            _logger.LogInformation("Channel is secure {frame.requestId}", frame.requestId);
                            continue;
                        }
                        _logger.LogWarning("Channel is not secure {frame.requestId} close the channel", frame.requestId);
                        break;
                    }

                    _ = Task.Run(async () =>
                    {
                        _logger.LogInformation("Execute {frame.requestId} request {frame.msgType} ", frame.requestId, frame.msgType);
                        try
                        {
                            await _executerManager.Execute(_channel, frame.msgType, frame.requestId, frame.payload, endpointId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,"Executer failed");
                        }                        
                    });
                }
                else
                {
                    _logger.LogInformation("Unknow message {frame.msgType} ", frame.msgType);
                }
            }//End of message loop 

            //Stop all event
            _eventDispatcher?.UnregisterAllEvents(_channel.ChannelId);
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
