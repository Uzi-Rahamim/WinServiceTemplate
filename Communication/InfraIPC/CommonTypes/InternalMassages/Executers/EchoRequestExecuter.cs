using Intel.IntelConnect.IPC.CommonTypes.Test;
using Intel.IntelConnect.IPC.Executer;
using Microsoft.Extensions.Logging;

namespace Intel.IntelConnect.IPC.CommonTypes.InternalMassages.Executers
{

    public class EchoRequestExecuter : SimpleRequestExecuter<EchoRequestExecuter, RequestEchoMessage, ResponseEchoMessage>
    {
        public EchoRequestExecuter(ILogger<EchoRequestExecuter> logger, CancellationTokenSource cts) : base(logger, cts) { }

        protected override Task<ResponseEchoMessage?> Execute(RequestEchoMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message;
            Logger.LogInformation("Server sent reply: {reply}", responseMsg);
            return Task.FromResult<ResponseEchoMessage?>(new ResponseEchoMessage(responseMsg));
        }
    }

}
