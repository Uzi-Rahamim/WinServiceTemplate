using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncPipe.Transport
{
    public class ServerTransportPipe : BaseTransportPipe, IServerTransport
    {
        NamedPipeServerStream pipeServer;
        public ServerTransportPipe(string pipeName)
        {
            pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }

        public void WaitForConnection()
        {

            PipeStream = pipeServer;
            // Wait for a client to connect
            pipeServer.WaitForConnection();
        }
    }
}