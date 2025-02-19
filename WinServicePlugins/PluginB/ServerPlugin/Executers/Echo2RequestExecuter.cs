using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using Service_BPlugin.Worker;
using Service_BPlugin.Contract.Massages;

namespace Service_BPlugin.Executers
{
    public class Echo2RequestExecuter : BaseRequestExecuter<Echo2RequestExecuter, RequestEcho2Message, ResponseEcho2Message>
    {
        Worker.SimpleWorker _simpleWorker;
        public Echo2RequestExecuter(ILogger<Echo2RequestExecuter> logger, CancellationTokenSource cts, SimpleWorker simpleWorker) : 
            base(logger, cts) {
            logger.LogInformation("Echo2RequestExecuter created");
            _simpleWorker = simpleWorker;
        }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return MessageType.Echo2;
        }

        protected override async Task<bool> Execute(RequestEcho2Message requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message+ " from Echo2RequestExecuter";
            //await Task.Delay(10000);
            await SendLastResponse(new ResponseEcho2Message(responseMsg));
            Logger.LogInformation("Server plugin sent reply: {reply} , WorkerMsg: {_simpleWorker.Message}", responseMsg, _simpleWorker.Message);
            _simpleWorker.SetChannel(Channel);
           
            return true;
        }
    }

}
