using AsyncPipeTransport.Executer;
using CommTypes.Massages;
using System.Reflection;

namespace App.WindowsService.API.Executers
{
    public class OpenSessionRequestExecuter : BaseRequestExecuter<OpenSessionRequestExecuter, RequestSecurityMessage, ResponseSecurityMessage>
    {
        public OpenSessionRequestExecuter(ILogger<OpenSessionRequestExecuter> logger, CancellationTokenSource cts, IServiceProvider serviceProvider) : base(logger, cts) { }

        protected override async Task<bool> Execute(RequestSecurityMessage requestMsg)
        {
            var hostVersion = Assembly.GetEntryAssembly()!.GetName().Version;
           
            var responseMsg = $"Security Request #{RequestId} version {hostVersion} Client Message : {requestMsg.token} ";
            
            bool isValid = (hostVersion != null);
            // Send a response back to the client
            await SendLastResponse(new ResponseSecurityMessage(isValid, 
                hostVersion?.ToString()??"Unknown host version"));
            return isValid;
        }
    }
}
