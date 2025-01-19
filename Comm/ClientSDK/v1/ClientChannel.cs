using AsyncPipeTransport.Channel;
using AsyncPipeTransport.ClientScheduler;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Events;
using CommunicationMessages;

namespace ClientSDK.v1
{
    public class ClientChannel : IDisposable
    {
        internal  ClientMessageScheduler Scheduler { get=>_clientTransportScheduler; }
        private readonly ClientMessageScheduler _clientTransportScheduler;

        public ClientChannel()
        {
            _clientTransportScheduler = new ClientMessageScheduler(new ClientPipeChannel(PipeApiConsts.PipeName));
        }

        public void Connect()
        {
            _clientTransportScheduler.Start();
        }

        public bool RegisterEvent(Opcode messageType, IEvent eventAction)
        {
            return _clientTransportScheduler.RegisterEvent(messageType, eventAction);
        }

        public void Dispose()
        {
            _clientTransportScheduler.Dispose();
        }
    }
}
