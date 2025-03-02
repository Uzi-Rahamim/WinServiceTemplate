using Microsoft.Extensions.Logging;

namespace PluginA.Worker
{
    public class SimpleWorker
    {
        ILogger<SimpleWorker> _logger;
        public string Message { get=> "SimpleWorker"; }
        public SimpleWorker(ILogger<SimpleWorker> logger)
        {
            _logger = logger;
            _logger.LogInformation("SimpleWorker created");
        }

        public void Start()
        {
            _logger.LogInformation("SimpleWorker started");
        }
    }
}
