﻿using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.CommonTypes
{

    public class EchoRequestExecuter : BaseRequestExecuter<EchoRequestExecuter, RequestEchoMessage, ResponseEchoMessage>
    {
        public EchoRequestExecuter(ILogger<EchoRequestExecuter> logger, CancellationTokenSource cts) : base(logger,cts) { }

        protected override async Task<bool> Execute(RequestEchoMessage requestMsg)
        {
            // Send a response back to the client
            var responseMsg = requestMsg.message;
            await SendLastResponse(new ResponseEchoMessage(responseMsg));
            Logger.LogInformation("Server sent reply: {reply}", responseMsg);

            return true;
        }
    }

}
