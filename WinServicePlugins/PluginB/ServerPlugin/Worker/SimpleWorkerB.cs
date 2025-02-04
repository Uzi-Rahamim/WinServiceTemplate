using Microsoft.Extensions.Logging;

namespace Service_ExecuterPlugin.Worker
{
    public class SimpleWorkerB
    {
        ILogger<SimpleWorkerB> _logger;
        public string Message { get=> "SimpleWorkerB"; }
        public SimpleWorkerB(ILogger<SimpleWorkerB> logger)
        {
            _logger = logger;
            _logger.LogInformation("SimpleWorkerB created");
        }

        public void Start()
        {
            _logger.LogInformation("SimpleWorkerB started");
        }
    }
}
