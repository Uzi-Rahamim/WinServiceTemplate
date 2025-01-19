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
