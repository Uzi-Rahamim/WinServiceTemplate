using Intel.IntelConnect.IPC.Executer;
using Intel.IntelConnect.IPC.v1.Executer;
using Microsoft.Extensions.Logging;
using PluginA.Contract.Massages;

namespace PluginA.Executers
{
    public class EchoRequestExecuter : SimpleRequestExecuter<EchoRequestExecuter, RequestEchoMessage, ResponseEchoMessage> , IRequestExecuter
    {
        public EchoRequestExecuter(ILogger<EchoRequestExecuter> logger, CancellationTokenSource cts) : 
            base(logger, cts) {
            logger.LogInformation("EchoRequestExecuter created");
        }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMethodName()
        {
            return MethodName.PluginA_Echo;
        }

        protected override Task<ResponseEchoMessage?> ExecuteAsync(RequestEchoMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message;
            Logger.LogInformation("Server plugin sent reply: {reply}", responseMsg);
            return Task.FromResult<ResponseEchoMessage?>(new ResponseEchoMessage(responseMsg));
        }
    }

}
