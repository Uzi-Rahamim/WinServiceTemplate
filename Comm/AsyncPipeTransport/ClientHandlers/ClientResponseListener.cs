using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;

namespace AsyncPipeTransport.ClientHandlers
{
    public class ClientResponseListener : IDisposable
    {
        private readonly IClientChannel _transport;
        private readonly IClientEventHandler _clientEventHandler;
        private readonly IClientRequestHandler _clientRequestHandler;
        Task? _listenerTask;
        bool _disposed = false;
        

        public ClientResponseListener(IClientChannel transport,
            IClientRequestHandler _clientRequestHandler,
            IClientEventHandler clientEventHandler)
        {
            this._clientRequestHandler = _clientRequestHandler; 
            this._clientEventHandler= clientEventHandler;
            this._transport = transport;
        }

        public Task Start()
        {
            _transport.Connect();

            _listenerTask = Task.Run(async () =>
            {
                while (!_disposed)
                {
                    // Receive the server's reply
                    string? messageStr = await _transport.ReceiveAsync();
                    if (messageStr == null)
                        break;

                    var frame = messageStr.ExtractFrameHeaders();
                    if (frame == null)
                    {
                        Console.WriteLine($"Client receive an invalid response message");
                        continue;
                    }
                    if (frame.options.HasFlag(FrameHeaderOptions.EvantMsg))
                    {
                        _clientEventHandler.HandleEvent(frame);
                    }
                    else if (_clientRequestHandler.GetPendingRequest(frame.requestId, out ClientRequest? request))
                    {
                        request?.PushResponse(frame);
                    }
                    else
                    {
                        Console.WriteLine($"Client no request found for this response {frame.requestId}");
                    }
                }
            });
            return _listenerTask;
        }
       
        public void Dispose()
        {
            _disposed = true;
            _transport.Dispose();
        }
    }

}
