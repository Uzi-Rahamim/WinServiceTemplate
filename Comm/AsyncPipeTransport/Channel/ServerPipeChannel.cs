using System.IO.Pipes;

namespace AsyncPipeTransport.Channel
{
    public class ServerPipeChannel : BasePipeChannel, IServerChannel
    {
        NamedPipeServerStream pipeServer;
        public ServerPipeChannel(string pipeName)
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