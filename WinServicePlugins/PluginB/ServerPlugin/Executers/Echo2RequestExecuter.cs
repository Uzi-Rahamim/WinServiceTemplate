using AsyncPipeTransport.Executer;
using Service_ExecuterPlugin.CommTypes.Massages;
using Microsoft.Extensions.Logging;
using Service_ExecuterPlugin.Worker;

namespace Service_ExecuterPlugin.Executers
{
    public class Echo2RequestExecuter : BaseRequestExecuter<Echo2RequestExecuter, RequestEcho2Message, ResponseEcho2Message>
    {
        Worker.SimpleWorkerB _simpleWorker;
        public Echo2RequestExecuter(ILogger<Echo2RequestExecuter> logger, CancellationTokenSource cts, SimpleWorkerB simpleWorker) : 
            base(logger, cts) {
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
            await SendLastResponse(new ResponseEcho2Message(responseMsg));
            Log.LogInformation("Server plugin sent reply: {reply} , WorkerMsg: {_simpleWorker.Message}", responseMsg, _simpleWorker.Message);

            return true;
        }
    }

}
