using AsyncPipeTransport.Executer;
using CommTypes.Massages;

namespace App.WindowsService.API.Executers
{
    public class OpenSessionRequestExecuter : BaseRequestExecuter<OpenSessionRequestExecuter, RequestSecurityMessage, ResponseSecurityMessage>
    {
        public OpenSessionRequestExecuter(ILogger<OpenSessionRequestExecuter> logger, CancellationTokenSource cts, IServiceProvider serviceProvider) : base(logger, cts) { }

        protected override async Task<bool> Execute(RequestSecurityMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = $"Security Request #{RequestId} Client Message : {requestMsg.token}";

            bool isValid = true;


            await SendLastResponse(new ResponseSecurityMessage(isValid));
            return isValid;
        }
    }
}
