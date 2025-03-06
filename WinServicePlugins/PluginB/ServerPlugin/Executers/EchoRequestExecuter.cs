using Microsoft.Extensions.Logging;
using PluginB.Worker;
using PluginB.Contract.Massages;
using Intel.IntelConnect.IPC.v1.Executer;
using Intel.IntelConnect.IPC.Attributes;

namespace PluginB.Executers
{
    [Executer<RequestEchoMessage, ResponseEchoMessage>(MethodName.PluginB_Echo)]
    public class EchoRequestExecuter : SimpleRequestExecuter<EchoRequestExecuter, RequestEchoMessage, ResponseEchoMessage>
    {
        Worker.SimpleWorker _simpleWorker;
        public EchoRequestExecuter(ILogger<EchoRequestExecuter> logger, CancellationTokenSource cts, SimpleWorker simpleWorker) :
            base(logger, cts)
        {
            logger.LogInformation("EchoRequestExecuter created");
            _simpleWorker = simpleWorker;
        }

        protected override Task<ResponseEchoMessage?> ExecuteAsync(RequestEchoMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message;
            //await Task.Delay(10000);
            Logger.LogInformation("Server plugin sent reply: {reply} , WorkerMsg: {_simpleWorker.Message}", responseMsg, _simpleWorker.Message);
            return Task.FromResult<ResponseEchoMessage?>(new ResponseEchoMessage(responseMsg));
        }
    }

}
