using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using Service_ExecuterPlugin.Worker;
using Service_48_ExecuterPlugin.CommTypes.Massages;
using OverClockingManager;

namespace Service_ExecuterPlugin.Executers
{
    public class GetPlatformCoampatibilityRequestExecuter : BaseRequestExecuter<Echo3RequestExecuter, RequestGetPlatformCompatibilityMessage, ResponseGetPlatformCompatibilityMessage>
    {
        Worker.SimpleWorker _simpleWorker;
        public GetPlatformCoampatibilityRequestExecuter(ILogger<Echo3RequestExecuter> logger, CancellationTokenSource cts, SimpleWorker simpleWorker) : 
            base(logger, cts) {
            _simpleWorker = simpleWorker;
        }

        public static string Plugin_GetSchema()
        {
            return GetSchema();
        }

        public static string Plugin_GetMessageType()
        {
            return MessageType.GetPlatformCompatibility;
        }

        protected override async Task<bool> Execute(RequestGetPlatformCompatibilityMessage requestMsg)
        {
            // Send a response back to the client
            //var responseMsg = requestMsg.message + " from Echo3RequestExecuter";
            var list = new List<string>();
            list.Add("List is empty");  
            //try
            //{
            //    IOverClockingLifeCycleManager OCMgr = OverClockingManagerFactory.CreateOverClockingLifeCycleManager();
            //    list = OCMgr.ListPlatformCompatibility();
            //}
            //catch (Exception ex)
            //{
            //    Logger.LogInformation(ex, "IOverClockingLifeCycleManager Error ");
            //}
            
            await SendLastResponse(new ResponseGetPlatformCompatibilityMessage(list));
            //Log.LogInformation("Server plugin sent reply: {reply} , WorkerMsg: {_simpleWorker.Message}", responseMsg, _simpleWorker.Message);

            return true;
        }
    }

}
