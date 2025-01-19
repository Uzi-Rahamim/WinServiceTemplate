using ClientSDK.v1;
using System;
using System.Threading.Tasks;

namespace SimpleClientApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var message = "test .Net Framework";

            using (var channel = new ClientChannel())
            {
                channel.Connect();


                var demoAPI = new DemoApi(channel);

                demoAPI.RegisterhPulsEvent((msg) => { Console.WriteLine($"Pulse Event {msg}"); });

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
