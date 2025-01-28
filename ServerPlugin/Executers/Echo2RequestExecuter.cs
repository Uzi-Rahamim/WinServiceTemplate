using AsyncPipeTransport.Executer;
using CommTypes.Massages;
using Microsoft.Extensions.Logging;

namespace Service_ExecuterPlugin.Executers
{
    public class Echo2RequestExecuter : BaseRequestExecuter<Echo2RequestExecuter, RequestEchoMessage>
    {
        public Echo2RequestExecuter(ILogger<Echo2RequestExecuter> logger) : base(logger) { }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return MessageType.Echo;
        }

        protected override async Task<bool> Execute(RequestEchoMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message;
            await SendLastResponse(new ResponseEchoMessage(responseMsg));
            Log.LogInformation("Server plugin sent reply: {reply}", responseMsg);

            return true;
        }
    }

}
