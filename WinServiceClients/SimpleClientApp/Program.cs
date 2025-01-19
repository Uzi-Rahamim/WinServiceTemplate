using ServerSDK.v1;
using System;
using System.Threading.Tasks;

namespace SimpleClientApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var message = "test .Net Framework";
            var testAPI = new TestAPI();
            var echoMsg = await testAPI.GetEcho(message);
            Console.WriteLine(($"Server 2 reply to {message} with: {echoMsg}"));

            //var networks = testAPI.GetAPList();
            //await foreach (var network in networks)
            //{
            //    Log.Information($"via SDK AP: {network.ssid} - {network.signalStrength}");
            //}
        }
    }
}
