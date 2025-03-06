using Intel.IntelConnect.IPC.Attributes;
using Intel.IntelConnect.IPC.Executer;
using Intel.IntelConnect.IPC.v1.Executer;
using Microsoft.Extensions.Logging;
using PluginA.Contract.Massages;

namespace PluginA.Executers
{
    [Executer<RequestEchoMessage, ResponseEchoMessage>(MethodName.PluginA_Echo)]
    public class EchoRequestExecuter : SimpleRequestExecuter<EchoRequestExecuter, RequestEchoMessage, ResponseEchoMessage> , IRequestExecuter
    {
        public EchoRequestExecuter(ILogger<EchoRequestExecuter> logger, CancellationTokenSource cts) : 
            base(logger, cts) {
            logger.LogInformation("EchoRequestExecuter created");
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
