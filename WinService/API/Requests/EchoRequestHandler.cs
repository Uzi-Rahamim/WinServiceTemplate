using AsyncPipeTransport.ServerHandlers;
using CommTypes.Events;
using CommunicationMessages.Massages;

namespace App.WindowsService.API.Requests
{

    public class EchoRequestHandler : BaseRequestCommand<EchoRequestHandler, RequestEchoMessage>
    {
        public EchoRequestHandler(ILogger<EchoRequestHandler> logger) : base(logger){}

        protected override async Task<bool> ExecuteInternal(RequestEchoMessage requestMsg)
        { 
            // Send a response back to the client
            var responseMsg =requestMsg.message;
            await SendLastResponse(new ResponseEchoMessage(responseMsg));
            Log.LogInformation("Server sent reply: {reply}", responseMsg);

            return true;
        }
    }
    
}
