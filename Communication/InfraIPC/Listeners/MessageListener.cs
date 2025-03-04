using Intel.IntelConnect.IPC.Channel;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Executer;
using Intel.IntelConnect.IPC.Extensions;
using Intel.IntelConnect.IPC.Request;
using Intel.IntelConnect.IPC.Events.Client;
using Intel.IntelConnect.IPC.Events.Service;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Intel.IntelConnect.IPC.Listeners
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

        public void StartListen(TimeSpan timeout)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("MessageListener Start {ChannelId} ...", _channel.ChannelId);
                    await StartReadMessageLoopAsync(timeout);
                    _logger.LogDebug("MessageListener Terminate");
                }
                catch (Exception ex) when (
                ex is TimeoutException ||
                ex is OperationCanceledException ||
                ex is IOException)
                {
                    _logger.LogDebug("MessageListener Terminate - {ChannelId} {type}", _channel.ChannelId, ex.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in MessageListener {ChannelId}", _channel.ChannelId);
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
                    _logger.LogDebug("PulseEvantGenerator {ChannelId} ...", _channel.ChannelId);
                    await StartPulseEvantGeneratorAsync();
                    _logger.LogDebug("PulseEvantGenerator Terminate");
                }
                catch (Exception ex) when (
                ex is TimeoutException || 
                ex is OperationCanceledException ||
                ex is IOException)
                {
                    _logger.LogDebug("PulseEvantGenerator Terminate {ChannelId} - {type} ", _channel.ChannelId, ex.GetType().Name);
                }
                catch (Exception ex)
                {   
                    if (_disposed)
                        _logger.LogDebug("PulseEvantGenerator Terminate {ChannelId} - {type} ", _channel.ChannelId, ex.GetType().Name);
                    else
                        _logger.LogError(ex, "Error in PulseEvantGenerator {ChannelId}", _channel.ChannelId);
                }
            });
        }

        private async Task StartReadMessageLoopAsync(TimeSpan timeout)
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
                    _logger.LogInformation("Receive an invalid message {ChannelId}", _channel.ChannelId);
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
                            channelIsSecure = await _executerManager.ExecuteAsync(_channel, FrameworkMethodName.OpenSession, frame.requestId, frame.payload);
                            if (!channelIsSecure)
                            {
                                _logger.LogWarning("Channel open session failed, close the channel {ChannelId}", _channel.ChannelId);
                                break;
                            }
                            _logger.LogInformation("Channel is secure {ChannelId} {frame.requestId}", _channel.ChannelId, frame.requestId);
                            continue;
                        }
                        _logger.LogWarning("Channel is not secure {frame.requestId} close the channel {ChannelId}", frame.requestId, _channel.ChannelId);
                        break;
                    }

                    _ = Task.Run(async () =>
                    {
                        _logger.LogInformation("Execute {frame.requestId} request {frame.methodName} channel {ChannelId}", frame.requestId, frame.methodName, _channel.ChannelId);
                        try
                        {
                            await _executerManager.ExecuteAsync(_channel, frame.methodName, frame.requestId, frame.payload);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Executer failed {ChannelId}", _channel.ChannelId);
                        }                        
                    });
                }
                else
                {
                    _logger.LogInformation("Unknow message {ChannelId} {frame.methodName} ", _channel.ChannelId, frame.methodName);
                }
            }//End of message loop 

            //Stop all event
            if (_eventDispatcher!=null)
                await _eventDispatcher.SafeUnregisterAllEventsAsync(_channel.ChannelId);
        }

        private async Task StartPulseEvantGeneratorAsync()
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
            _logger.LogDebug("Disposed {ChannelId}", _channel.ChannelId);
            _disposed = true;
            _channel.Dispose();
        }
    }

}
