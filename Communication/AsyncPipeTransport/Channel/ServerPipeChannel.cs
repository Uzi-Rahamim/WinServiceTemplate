using AsyncPipeTransport.Extensions;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace AsyncPipeTransport.Channel
{
    public class ServerPipeChannel : BasePipeChannel, IServerChannel
    {
        NamedPipeServerStream _pipeServer;
        private  const int _bufferSize = 2024;
        public ServerPipeChannel(string pipeName)
        {
            var networkSid = new SecurityIdentifier(WellKnownSidType.NetworkSid, null);
            var pipeSecurity = new PipeSecurity();

            pipeSecurity.AddAccessRule(new PipeAccessRule(networkSid, PipeAccessRights.FullControl,
                AccessControlType.Deny));

            
            pipeSecurity.AddAccessRule(new PipeAccessRule(
                new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), 
                PipeAccessRights.ReadWrite,
                AccessControlType.Allow));
            
            pipeSecurity.AddAccessRule(new PipeAccessRule(
                new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),  // SID for built-in users
                PipeAccessRights.FullControl,
                AccessControlType.Allow));

#if NET8_0_OR_GREATER
            _pipeServer = NamedPipeServerStreamAcl.Create(
                pipeName, 
                PipeDirection.InOut, 
                NamedPipeServerStream.MaxAllowedServerInstances, 
                PipeTransmissionMode.Message, 
                PipeOptions.Asynchronous ,
                _bufferSize, _bufferSize, pipeSecurity);
#elif NETFRAMEWORK
            _pipeServer = new NamedPipeServerStream(
                pipeName, 
                PipeDirection.InOut, 
                NamedPipeServerStream.MaxAllowedServerInstances, 
                PipeTransmissionMode.Message, 
                PipeOptions.Asynchronous,
                _bufferSize,_bufferSize,pipeSecurity);
#endif
        }

        public Task WaitForConnectionAsync(CancellationToken cancellationToken)
        {

            PipeStream = _pipeServer;
            // Wait for a client to connect
            return _pipeServer.WaitForConnectionAsync(cancellationToken);
        }

        public int GetConnectedProcessId() =>
            _pipeServer?.GetNamedPipeClientProcessId() ?? throw new Exception();
    }
}