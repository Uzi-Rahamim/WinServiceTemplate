using AsyncPipeTransport.Extensions;
using System;
using System.IO.Pipes;

namespace AsyncPipeTransport.Channel
{
    public class ClientPipeChannel : BasePipeChannel, IClientChannel
    {
        NamedPipeClientStream _pipeClient;
        public ClientPipeChannel(string pipeName)
        {
            _pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        }
        public async Task ConnectAsync(TimeSpan timeout)
        {
            // Connect to the server
            PipeStream = _pipeClient;
            await _pipeClient.ConnectAsync((int)timeout.TotalMilliseconds);
            // pipeClient.ReadMode = PipeTransmissionMode.Message;
        }

        public int GetNamedPipeServerProcessId() =>
           _pipeClient?.GetNamedPipeServerProcessId() ?? throw new Exception();
    }

}
