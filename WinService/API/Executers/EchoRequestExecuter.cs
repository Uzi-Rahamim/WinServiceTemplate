using AsyncPipeTransport.Executer;
using CommTypes.Massages;

namespace App.WindowsService.API.Executers
{

    public class EchoRequestExecuter : BaseRequestExecuter<EchoRequestExecuter, RequestEchoMessage, ResponseEchoMessage>
    {
        public EchoRequestExecuter(ILogger<EchoRequestExecuter> logger,CancellationTokenSource cts) : base(logger,cts) { }

        protected override async Task<bool> Execute(RequestEchoMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message;
            await SendLastResponse(new ResponseEchoMessage(responseMsg));
            Log.LogInformation("Server sent reply: {reply}", responseMsg);

            return true;
        }
    }

}
