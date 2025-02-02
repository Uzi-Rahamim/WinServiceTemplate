using System;
using System.IO.Pipes;

namespace AsyncPipeTransport.Channel
{
    public class ClientPipeChannel : BasePipeChannel, IClientChannel
    {
        NamedPipeClientStream pipeClient;
        public ClientPipeChannel(string pipeName)
        {
            pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        }
        public async Task ConnectAsync(TimeSpan timeout)
        {
            // Connect to the server
            PipeStream = pipeClient;
            await pipeClient.ConnectAsync((int)timeout.TotalMilliseconds);
            // pipeClient.ReadMode = PipeTransmissionMode.Message;
        }
    }

}
