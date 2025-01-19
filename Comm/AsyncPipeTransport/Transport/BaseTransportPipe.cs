using AsyncPipeTransport.Transport;
using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPipe.Transport
{
    public abstract class BaseTransportPipe : ITransportSender
    {

        bool disposed = false;
        protected PipeStream PipeStream { get; set; }

        protected BaseTransportPipe()
        {
            PipeStream = default!;
        }

        public Task SendAsync(string message)
        {
            lock (PipeStream)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] dwordBytes = BitConverter.GetBytes((uint)messageBytes.Length);
                PipeStream.Write(dwordBytes, 0, dwordBytes.Length);

                PipeStream.Write(messageBytes, 0, messageBytes.Length);
                return PipeStream.FlushAsync();
            }
        }

        public async Task<string?> ReceiveAsync()
        {
            try
            {
                // Read the message length
                byte[] dwordBytes = new byte[4];
                PipeStream.Read(dwordBytes, 0, dwordBytes.Length);
                uint len = BitConverter.ToUInt32(dwordBytes, 0);
                if (len <= 0 || disposed)
                    return null;

                // Read message body
                byte[] buffer = new byte[len];
                int bytesRead = await PipeStream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    return null;

                string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                return serverResponse;
            }
            catch (Exception)
            {
                if (disposed)
                    return null;
                throw;
            }
        }

        public string? Receive()
        {
            try
            {
                var len = PipeStream.ReadByte();  // Read the message length
                if (len <= 0 || disposed)
                    return null;

                byte[] buffer = new byte[len];
                int bytesRead = PipeStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    return null;

                string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                return serverResponse;
            }
            catch (Exception)
            {
                if (disposed)
                    return null;
                throw;
            }
        }

        public void Dispose()
        {
            disposed = true;
            PipeStream.Dispose();
        }

       
    }
}