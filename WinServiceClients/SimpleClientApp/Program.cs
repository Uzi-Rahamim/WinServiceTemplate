using ClientSDK.v1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SimpleClientApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (var channel = new ClientChannel())
            {
                if (!await channel.Connect())
                {
                    Console.WriteLine("Failed to connect to server");
                    return;
                }


                var demoAPI = new DemoApi(channel);

                //demoAPI.RegisterPulsEvent((msg) => { Console.WriteLine($"Pulse Event {msg}"); });
                var message = "Hello from client";
                var echoMsg = await demoAPI.GetEcho(message);
                Console.WriteLine(($"Server 2 reply to {message} with: {echoMsg}"));

                await demoAPI.GetAPListStream((network) =>
                     Console.WriteLine($"via SDK AP: {network.ssid} - {network.signalStrength}")
                     );


                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
