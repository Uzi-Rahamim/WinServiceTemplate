//using AsyncPipe.Transport;
//using AsyncPipeTransport.CommonTypes;
//using Microsoft.Extensions.Logging;

//namespace ServiceAPI;
//public class ServerMessageScheduler
//{
//    private long m_clientId = 0;
//    private readonly ILogger<ServerMessageScheduler> _logger;
//    private readonly Dictionary<Opcode, IRequestHandler> commands = new Dictionary<Opcode, IRequestHandler>();

//    public ServerMessageScheduler(ILogger<ServerMessageScheduler> logger, IEnumerable<IRequestHandler> cmdList)
//    {
//        _logger = logger;
//        foreach (var cmd in cmdList)
//        {
//            var x = cmd.GetMessageType();
//            commands.Add(cmd.GetMessageType(), cmd);
//        }
//    }

//    public Task Start(string pipeName)
//    {
//        return Task.Run(()=>StartListen(pipeName));
//    }

//    private void StartListen(string pipeName)
//    {
//        while (true)
//        {
//            ManualResetEvent signal = new ManualResetEvent(false);
//            _ = Task.Run(async () =>
//            {
//                var clientId = Interlocked.Increment(ref m_clientId);
//                // Create a NamedPipeServerStream to listen for connections
//                using (IServerTransport pipeServer = new ServerTransportPipe(pipeName))
//                {
//                    _logger.LogInformation("Server {clientId}  Waiting for a client to connect...", clientId);

//                    // Wait for a client to connect
//                    pipeServer.WaitForConnection();
//                    _logger.LogInformation("Server {clientId}  Client connected.", clientId);

//                    if (pipeServer == null)
//                        return;

//                    signal.Set();
//                    await HandleClient(pipeServer, clientId);

//                    _logger.LogInformation("Server {clientId}  Exit.", clientId);
//                }
//            });
//            signal.WaitOne(); // Block until signaled
//        }
//    }

//    async Task HandleClient(IServerTransport pipeServer, long clientId)
//    {
//        while (true)
//        {
//            string? clientMessage = await pipeServer.ReceiveAsync();
//            if (clientMessage == null)
//                break;

//            var frame = clientMessage.ExtractFrameHeaders();
//            if (frame == null)
//            {
//                _logger.LogInformation("Server {clientId} received an invalid request message", clientId);
//                continue;
//            }
//            _ = Task.Run(async () =>
//            {
//                var cmd = GetCommand(frame, clientId);
//                if (cmd == null)
//                {
//                    _logger.LogInformation("Server {clientId} command handler not found {frame.requestId} ", clientId, frame.requestId);
//                    return;
//                }
//                await cmd.Execute(pipeServer, frame.requestId, frame.payload);
//            });

//            _logger.LogInformation("Server {clientId} received request: {frame.requestId} ", clientId, frame.requestId);
//        }
//    }

//    private IRequestHandler? GetCommand(TransportFrameHeader frame, long clientId)
//    {
//        if (!commands.ContainsKey(frame.msgType))
//        {
//            _logger.LogInformation("Server {clientId} command not found {frame.msgType}", clientId, frame.requestId);
//            return null;
//        }
//        return commands[frame.msgType];
//    }
//}

