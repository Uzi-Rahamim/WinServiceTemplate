using Intel.IntelConnect.IPC.Extensions;
using Microsoft.Extensions.Logging;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Intel.IntelConnect.IPC.Channel
{
    public class ServerPipeChannel : BasePipeChannel, IServerChannel
    {
        private readonly NamedPipeServerStream _pipeServer;
        private  const int _bufferSize = 2024;
        public ServerPipeChannel(ILogger<ClientPipeChannel> logger,string pipeName) : base(logger)
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
                PipeTransmissionMode.Byte, 
                PipeOptions.Asynchronous ,
                _bufferSize, _bufferSize, pipeSecurity);
#elif NETFRAMEWORK
            _pipeServer = new NamedPipeServerStream(
                pipeName, 
                PipeDirection.InOut, 
                NamedPipeServerStream.MaxAllowedServerInstances, 
                PipeTransmissionMode.Byte, 
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