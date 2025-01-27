using AsyncPipeTransport.Channel;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using System.Threading.Tasks;

namespace AsyncPipeTransport.ClientHandlers
{
    public class MessageListener : IDisposable
    {
        private readonly IClientChannel _channel;
        private readonly IClientEventManager _clientEventHandler;
        private readonly IClientRequestManager _clientRequestHandler;
        Task? _listenerTask;
        bool _disposed = false;
        

        public MessageListener(IClientChannel channel,
            IClientRequestManager _clientRequestHandler,
            IClientEventManager clientEventHandler)
        {
            this._clientRequestHandler = _clientRequestHandler; 
            this._clientEventHandler= clientEventHandler;
            this._channel = channel;
        }

        public async Task<bool> StartAsync(TimeSpan timeout)
        {
            try
            {
                await _channel.ConnectAsync(timeout);
            }
            catch (TimeoutException)
            {
                return false;
            }

            _listenerTask = Task.Run(async () =>
            {
                while (!_disposed)
                {
                    // Receive the server's reply
                    string? messageStr = await _channel.ReceiveAsync();
                    if (messageStr == null)
                        break;

                    var frame = messageStr.ExtractFrameHeaders();
                    if (frame == null)
                    {
                        Console.WriteLine($"Client receive an invalid response message");
                        continue;
                    }
                    else if (frame.options.HasFlag(FrameOptions.EvantMsg))
                    {
                        _clientEventHandler.HandleEvent(frame);
                    }
                    else if (frame.options.HasFlag(FrameOptions.Response) && _clientRequestHandler.GetPendingRequest(frame.requestId, out ClientRequest? request))
                    {
                        request?.PushResponse(frame);
                    }
                    else
                    {
                        Console.WriteLine($"Client no request found for this response {frame.requestId}");
                    }
                }
            });
            //await _listenerTask;    
            return true;
        }
       
        public void Dispose()
        {
            _disposed = true;
            _channel.Dispose();
        }
    }

}
