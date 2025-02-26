using AsyncPipeTransport.Executer;
using CommTypes.Massages;
using System.Reflection;

namespace App.WindowsService.API.Executers
{
    public class OpenSessionRequestExecuter : SimpleRequestExecuter<OpenSessionRequestExecuter, RequestSecurityMessage, ResponseSecurityMessage>
    {
        public OpenSessionRequestExecuter(ILogger<OpenSessionRequestExecuter> logger, CancellationTokenSource cts, IServiceProvider serviceProvider) : base(logger, cts) { }

        protected override async Task<ResponseSecurityMessage?> Execute(RequestSecurityMessage requestMsg)
        {
            var hostVersion = Assembly.GetEntryAssembly()!.GetName().Version;

            var responseMsg = $"Security version {hostVersion} Client Message : {requestMsg.token} ";

            bool isValid = (hostVersion != null);

            // Simulate async work
            await Task.Yield();

            // Send a response back to the client
            return new ResponseSecurityMessage(isValid,
                hostVersion?.ToString() ?? "Unknown host version");
        }
    }
}
