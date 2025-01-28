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

    internal class MessageListener : IDisposable
    {
        protected readonly IChannel _channel;
        private readonly IClientEventManager? _clientEventHandler;
        private readonly IClientRequestManager? _clientRequestHandler;
        private readonly IExecuterManager? _executerManager;
        private readonly IClientsManager? _activeClients;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger _logger;
        private bool _disposed = false;


        public MessageListener(
            ILogger logger,
            CancellationToken cancellationToken,
            IChannel channel,
            IClientRequestManager? clientRequestHandler,
            IClientEventManager? clientEventHandler,
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

        public void StartReadMessageLoop(TimeSpan timeout, long endpointId)
        {
            _ = Task.Run(async () =>
            {
                bool channelIsSecure = false;
                while (!_disposed)
                {
                    string? messageStr = await _channel.ReceiveAsync();
                    if (messageStr == null)
                        break;

                    var frame = messageStr.ExtractFrameHeaders();
                    if (frame == null)
                    {
                        _logger.LogInformation($"Receive an invalid message");
                        continue;
                    }
                    else if (frame.IsEventFrame() && _clientEventHandler != null)
                    {
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
                            continue;
                        }
                        _ = Task.Run(async () =>
                        {
                            await _executerManager.Execute(_channel, frame.msgType, frame.requestId, frame.payload, endpointId);
                        });
                        _logger.LogInformation("Client no request {frame.requestId} found for this response", frame.requestId);
                    }
                    else
                    {
                        _logger.LogInformation($"Client no request {frame.requestId} found for this response", frame.requestId);
                    }
                }
                //_activeClients.RemoveClient(clientId);
            });
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _channel.Dispose();
        }
    }

}
