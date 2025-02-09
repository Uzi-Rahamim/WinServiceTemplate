using AsyncPipeTransport.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.IO.Pipes;

namespace AsyncPipeTransport.Channel
{
    public class ClientPipeChannel : BasePipeChannel, IClientChannel
    {
        NamedPipeClientStream _pipeClient;
        public ClientPipeChannel(ILogger<ClientPipeChannel> logger, string pipeName): base(logger)
        {
            _pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        }
        public async Task ConnectAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            // Connect to the server
            PipeStream = _pipeClient;
            await _pipeClient.ConnectAsync((int)timeout.TotalMilliseconds);
            //StartMonitor(cancellationToken);
            // pipeClient.ReadMode = PipeTransmissionMode.Message;
        }

        public int GetNamedPipeServerProcessId() =>
           _pipeClient?.GetNamedPipeServerProcessId() ?? throw new Exception();
    }

}
