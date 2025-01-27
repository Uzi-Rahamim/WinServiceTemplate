using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.ServerHandlers;
using CommunicationMessages;
using CommunicationMessages.Massages;
using Microsoft.Extensions.Logging;

namespace ServerPlugin
{
    public class Echo2RequestHandler : BaseRequestExecuter<Echo2RequestHandler, RequestEchoMessage> 
    {
        public Echo2RequestHandler(ILogger<Echo2RequestHandler> logger) : base(logger){}

        public static string Plugin_GetSchema() {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return MessageType.Echo;
        }

        protected override async Task<bool> ExecuteInternal(RequestEchoMessage requestMsg)
        { 
            // Send a response back to the client
            var responseMsg =requestMsg.message;
            await SendLastResponse(new ResponseEchoMessage(responseMsg));
            Log.LogInformation("Server plugin sent reply: {reply}", responseMsg);

            return true;
        }
    }
    
}
