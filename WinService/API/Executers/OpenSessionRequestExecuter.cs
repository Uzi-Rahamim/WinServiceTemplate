using Intel.IntelConnect.IPC.Attributes;
using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.v1.Executer;
using Intel.IntelConnect.IPCCommon.Massages;
using System.Reflection;

namespace Intel.IntelConnect.WindowsService.API.Executers
{
    [Executer<RequestSecurityMessage, ResponseSecurityMessage>(FrameworkMethodName.OpenSession)]
    public class OpenSessionRequestExecuter : SimpleRequestExecuter<OpenSessionRequestExecuter, RequestSecurityMessage, ResponseSecurityMessage>
    {
        public OpenSessionRequestExecuter(ILogger<OpenSessionRequestExecuter> logger, CancellationTokenSource cts, IServiceProvider serviceProvider) : base(logger, cts) { }

        protected override async Task<ResponseSecurityMessage?> ExecuteAsync(RequestSecurityMessage requestMsg)
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
