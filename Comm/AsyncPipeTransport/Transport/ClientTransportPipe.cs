using System;
using System.IO.Pipes;

namespace AsyncPipe.Transport
{
    public class ClientTransportPipe : BaseTransportPipe, IClientTransport
    {
        NamedPipeClientStream pipeClient;
        public ClientTransportPipe(string pipeName)
        {
            pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        }
        public void Connect()
        {
            // Connect to the server
            Console.WriteLine("Client connecting...");
            pipeClient.Connect();
            PipeStream = pipeClient;
            Console.WriteLine("Client Connected.");
        }
    }

}
