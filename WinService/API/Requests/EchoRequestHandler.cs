using AsyncPipeTransport.ServerScheduler;
using CommTypes.Events;
using CommunicationMessages.Massages;

namespace App.WindowsService.API.Requests
{

    public class EchoRequestHandler : BaseRequestHandler<EchoRequestHandler, RequestEchoMessage>
    {
        public EchoRequestHandler(ILogger<EchoRequestHandler> logger) : base(logger){}

        protected override async Task ExecuteInternal(RequestEchoMessage requestMsg)
        { 
            // Send a response back to the client
            var responseMsg = $"Request #{RequestId} Client Message : {requestMsg.message}";
            await SendLastResponse(new ResponseEchoMessage(responseMsg));
            await SendEvent(new PulseEventMessage("PulseEvent"));
            Log.LogInformation("Server sent reply: {reply}", responseMsg);
        }
    }
    
}
