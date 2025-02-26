using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using PluginB.Worker;
using PluginB.Contract.Massages;

namespace PluginB.Executers
{
    public class EchoRequestExecuter : SimpleRequestExecuter<EchoRequestExecuter, RequestEchoMessage, ResponseEchoMessage>
    {
        Worker.SimpleWorker _simpleWorker;
        public EchoRequestExecuter(ILogger<EchoRequestExecuter> logger, CancellationTokenSource cts, SimpleWorker simpleWorker) :
            base(logger, cts)
        {
            logger.LogInformation("EchoRequestExecuter created");
            _simpleWorker = simpleWorker;
        }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return MessageType.PluginB_Echo;
        }

        protected override Task<ResponseEchoMessage?> Execute(RequestEchoMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message;
            //await Task.Delay(10000);
            Logger.LogInformation("Server plugin sent reply: {reply} , WorkerMsg: {_simpleWorker.Message}", responseMsg, _simpleWorker.Message);
            return Task.FromResult<ResponseEchoMessage?>(new ResponseEchoMessage(responseMsg));
        }
    }

}
