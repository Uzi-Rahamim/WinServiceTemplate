using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using PluginA.Contract.Massages;

namespace PluginA.Executers
{
    public class EchoRequestExecuter : BaseRequestExecuter<EchoRequestExecuter, RequestEchoMessage, ResponseEchoMessage> , IRequestExecuter
    {
        public EchoRequestExecuter(ILogger<EchoRequestExecuter> logger, CancellationTokenSource cts) : 
            base(logger, cts) {
            logger.LogInformation("Echo3RequestExecuter created");
        }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return MessageType.PluginA_Echo;
        }

        protected override async Task<bool> Execute(RequestEchoMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message + " from EchoRequestExecuter";
            await SendLastResponse(new ResponseEchoMessage(responseMsg));
            Logger.LogInformation("Server plugin sent reply: {reply}", responseMsg);

            return true;
        }
    }

}
