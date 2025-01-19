using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.RequestHandler;
using CommunicationMessages;
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
            Log.LogInformation("Server sent reply: {reply}", responseMsg);
        }
    }
    
}
