using AsyncPipeTransport.CommonTypes;
using Microsoft.Extensions.Logging;
using System.IO.Pipes;
using System.Text;

namespace AsyncPipeTransport.Channel
{
    public abstract class BasePipeChannel : IChannel
    {
        public event Action? OnDisconnect;
        public Guid ChannelId { get => Guid.NewGuid(); }

        private bool _disposed = false;
        private DateTime _lastMessageTimeStamp = DateTime.UtcNow;
        private readonly ILogger _logger;
        
        //BufferPool<BytesBufferItem> _buffPool;
        protected PipeStream PipeStream { get; set; }


        protected BasePipeChannel(ILogger logger)
        {
            _logger = logger;
            PipeStream = default!;
            //_buffPool = new BufferPool<BytesBufferItem>(10, (returnItem) => new BytesBufferItem(1024, returnItem as Action<BaseBufferItem>));
        }

        private void OnDisconnectInternal()
        {
            var disconnectEvent = OnDisconnect;
            disconnectEvent?.Invoke();
        }

        public Task SendAsync(string message, CancellationToken cancellationToken)
        {
            lock (PipeStream)
            {
                try
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    byte[] dwordBytes = BitConverter.GetBytes((uint)messageBytes.Length);
                    PipeStream.Write(dwordBytes, 0, dwordBytes.Length); //use in byte mode

                    PipeStream.Write(messageBytes, 0, messageBytes.Length);
                    return PipeStream.FlushAsync(cancellationToken);
                }
                catch (IOException)
                {
                    OnDisconnectInternal();
                    throw;
                }
            }
        }

        public async Task<string?> ReceiveAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Read the message length 
                byte[] dwordBytes = new byte[4];
                await PipeStream.ReadAsync(dwordBytes, 0, dwordBytes.Length, cancellationToken);
                uint len = BitConverter.ToUInt32(dwordBytes, 0);
                if (len <= 0 || _disposed)
                    return null;

                // Read message body
                byte[] buffer = new byte[len];
                //byte[] buffer = new byte[Consts.MaxMessageSize];
                int bytesRead = await PipeStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead == 0)
                    return null;

                _lastMessageTimeStamp = DateTime.UtcNow;
                string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                return serverResponse;
            }
            catch (IOException)
            {
                _logger.LogDebug("ReceiveAsync IOException");
                OnDisconnectInternal();
                if (_disposed)
                    return null;
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"ReceiveAsync Exception");
                if (_disposed)
                    return null;
                throw;
            }
        }

        public bool IsConnected()
        {
            if (_disposed)
                return false;

            var isConnected = PipeStream?.IsConnected ?? false;
            if (isConnected && _lastMessageTimeStamp < DateTime.UtcNow.AddMilliseconds(-Consts.MaxConnectionMonitorInterval*2))
            {
                _logger.LogInformation("IsConnected false");
                isConnected = false;
            }
            return isConnected;
        }

        //protected void StartMonitor(CancellationToken cancellationToken)
        //{
        //    _ = Task.Run(async () =>
        //    {
        //        while (!cancellationToken.IsCancellationRequested)
        //        {
        //            Console.WriteLine("Monitor");
        //            await Task.Delay(TimeSpan.FromMilliseconds(Consts.MaxConnectionMonitorInterval), cancellationToken);
        //            if (!IsConnected())
        //            {
        //                OnDisconnectInternal();
        //                break;
        //            }
        //        }
        //    });
        //}

        public void Dispose()
        {
            if (_disposed)
                return;
            _logger.LogDebug("Dispose");
            _disposed = true;
            PipeStream.Dispose();
        }
    }
}