using AsyncPipeTransport.Executer;
using CommTypes.Massages;
using Microsoft.Extensions.Logging;
using Service_ExecuterPlugin.Worker;

namespace Service_ExecuterPlugin.Executers
{
    public class Echo2RequestExecuter : BaseRequestExecuter<Echo2RequestExecuter, RequestEchoMessage, ResponseEchoMessage>
    {
        Worker.SimpleWorker _simpleWorker;
        public Echo2RequestExecuter(ILogger<Echo2RequestExecuter> logger, SimpleWorker simpleWorker) : base(logger) {
            _simpleWorker = simpleWorker;
        }

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
            Log.LogInformation("Server plugin sent reply: {reply} , WorkerMsg: {_simpleWorker.Message}", responseMsg, _simpleWorker.Message);

            return true;
        }
    }

}
