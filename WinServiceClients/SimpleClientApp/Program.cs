using Intel.IntelConnect.ClientSDK.v1;
using Microsoft.Extensions.Logging;
using PluginA.ClientSDK.v1;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleClientApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                //builder.AddConsole(); // This will log to the console
            });

            try
            {
                using (var channel = new SdkClientChannel(loggerFactory))
                {
                    if (!await channel.Connect())
                    {
                        Console.WriteLine("Failed to connect to server");
                        return;
                    }


                    var winServiceApi = new WinServiceApi(channel);

                    //demoAPI.RegisterPulsEvent((msg) => { Console.WriteLine($"Pulse Event {msg}"); });
                    var message = "Hello from client";
                    var echoMsg = await winServiceApi.GetEcho(message);
                    Console.WriteLine(($"Server 2 reply to {message} with: {echoMsg}"));

                    var pluginA_Api = new PluginA_Api(channel);
                    await pluginA_Api.GetAPListStream((network) =>
                         Console.WriteLine($"via SDK AP: {network.ssid} - {network.signalStrength}")
                         );


                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                }
            }
            catch (Exception ex) when (
                    ex is TimeoutException ||
                    ex is OperationCanceledException ||
                    ex is IOException)
            {
                Console.WriteLine($"Exception {ex.GetType().Name} in MessageListener ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MessageListener {ex}");
            }
        }
    }
}
