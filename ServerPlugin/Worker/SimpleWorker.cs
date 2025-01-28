using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_ExecuterPlugin.Worker
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
