using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using Service_APlugin.Worker;
using Service_APlugin.Contract.Massages;

namespace Service_APlugin.Executers
{
    public class Echo3RequestExecuter : BaseRequestExecuter<Echo3RequestExecuter, RequestEcho3Message, ResponseEcho3Message>
    {
        Worker.SimpleWorker _simpleWorker;
        public Echo3RequestExecuter(ILogger<Echo3RequestExecuter> logger, CancellationTokenSource cts, SimpleWorker simpleWorker) : 
            base(logger, cts) {
            _simpleWorker = simpleWorker;
        }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return MessageType.Echo3;
        }

        protected override async Task<bool> Execute(RequestEcho3Message requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message + " from Echo3RequestExecuter";
            await SendLastResponse(new ResponseEcho3Message(responseMsg));
            Log.LogInformation("Server plugin sent reply: {reply} , WorkerMsg: {_simpleWorker.Message}", responseMsg, _simpleWorker.Message);

            return true;
        }
    }

}
