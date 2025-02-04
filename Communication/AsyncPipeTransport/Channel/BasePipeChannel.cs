using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Utils;
using System.IO.Pipes;
using System.Text;

namespace AsyncPipeTransport.Channel
{
    public abstract class BasePipeChannel : IChannel
    {

        bool _disposed = false;
        //BufferPool<BytesBufferItem> _buffPool;
        protected PipeStream PipeStream { get; set; }

        protected BasePipeChannel()
        {
            PipeStream = default!;
            //_buffPool = new BufferPool<BytesBufferItem>(10, (returnItem) => new BytesBufferItem(1024, returnItem as Action<BaseBufferItem>));
        }

        public Task SendAsync(string message, CancellationToken cancellationToken)
        {
            lock (PipeStream)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] dwordBytes = BitConverter.GetBytes((uint)messageBytes.Length);
                PipeStream.Write(dwordBytes, 0, dwordBytes.Length); //use in byte mode

                PipeStream.Write(messageBytes, 0, messageBytes.Length);
                return PipeStream.FlushAsync(cancellationToken);
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

                string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                return serverResponse;
            }
            catch (Exception)
            {
                if (_disposed)
                    return null;
                throw;
            }
        }

        public bool IsConnected()
        {
            return PipeStream?.IsConnected ?? false;
        }

        public void Dispose()
        {
            _disposed = true;
            PipeStream.Dispose();
        }
    }
}