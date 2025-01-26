using AsyncPipeTransport.ServerHandlers;
using CommTypes.Massages;

namespace App.WindowsService.API.Requests
{
    public class OpenSessionRequestHandler : BaseRequestExecuter<OpenSessionRequestHandler, RequestSecurityMessage>
    {
        public OpenSessionRequestHandler(ILogger<OpenSessionRequestHandler> logger, IServiceProvider serviceProvider) : base(logger) { }

        protected override async Task<bool> ExecuteInternal(RequestSecurityMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = $"Security Request #{RequestId} Client Message : {requestMsg.token}";

            bool isValid = true;


            await SendLastResponse(new ResponseSecurityMessage(isValid));
            return isValid;
        }
    }
}
