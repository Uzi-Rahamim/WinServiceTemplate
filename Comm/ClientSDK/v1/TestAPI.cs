using AsyncPipe.Transport;
using CommunicationMessages;
using CommunicationMessages.Massages;
using Microsoft.Extensions.Logging;
using ServerSDK.CommonTypes;
using ServerSDK.Convertors;
using AsyncPipeTransport.ClientDistributer;
using AsyncPipeTransport.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerSDK.v1
{
    public class TestAPI
    {
        //private readonly ILogger<TestAPI> _logger;

        //public TestAPI(ILogger<TestAPI> logger)
        //{
        //    _logger = logger;
        //}

        //public async Task<IEnumerable<WiFiNetwork>> GetAPList2()
        //{
        //    using (var clientTransportScheduler = new ClientMessageScheduler(new ClientTransportPipe(PipeApiConsts.PipeName)))
        //    {
        //        _ = clientTransportScheduler.Start();
        //        var responses = clientTransportScheduler.SendLongRequest<RespnseWiFiNetworksMessage>((requestId) => (new RequestWiFiNetworksMessage()).BuildRequestMessage(requestId));

        //        //int pageCounter = 1;
        //        await foreach (var response in responses)
        //        {
        //            //Log.LogInformation($"AP Page {pageCounter++}");
        //            foreach (var network in response.list)
        //            {
        //                if (network == null)
        //                    continue;
        //                yield return network.ToWiFiNetwork();
        //                //_logger.LogInformation($"AP: {item.ssid} - {item.signalStrength}");
        //            }
        //        }
        //    }
        //}

        public async IAsyncEnumerable<WiFiNetwork> GetAPList()
        {
            using (var clientTransportScheduler = new ClientMessageScheduler(new ClientTransportPipe(PipeApiConsts.PipeName)))
            {
                _ = clientTransportScheduler.Start();
                var responses = clientTransportScheduler.SendLongRequest<RespnseWiFiNetworksMessage>((requestId) => (new RequestWiFiNetworksMessage()).BuildRequestMessage(requestId));

                //int pageCounter = 1;
                await foreach (var response in responses)
                {
                    //Log.LogInformation($"AP Page {pageCounter++}");
                    foreach (var network in response.list)
                    {
                        if (network == null)
                            continue;
                        yield return network.ToWiFiNetwork();
                        //_logger.LogInformation($"AP: {item.ssid} - {item.signalStrength}");
                    }
                }
            }
        }

        public async Task<string?> GetEcho(string message)
        {
            using (var clientRequestScheduler = new ClientMessageScheduler(new ClientTransportPipe(PipeApiConsts.PipeName)))
            {
                _ = clientRequestScheduler.Start();
                var reply = await clientRequestScheduler.Send((requestId) => (new RequestEchoMessage(message)).BuildRequestMessage(requestId));

                if (reply == null)
                {
                    //_logger.LogError("Client no reply received - Timeout.");
                    return null;
                }

                var response = reply.ExtractMessageHeaders<ResponseEchoMessage>();
                return response?.message;
                //_logger.LogInformation($"Server 2 reply to {message} with: {response?.message}");

            }
        }
    }
}