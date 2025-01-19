using AsyncPipe.Transport;
using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncPipeTransport.ClientDistributer
{
    public class ClientMessageScheduler : IDisposable
    {
        IClientTransport transport;
        Task? listenerTask;
        bool disposed = false;
        ConcurrentDictionary<long, Request> pendingRequests = new ConcurrentDictionary<long, Request>();
        ConcurrentDictionary<Opcode, IEvent> events = new ConcurrentDictionary<Opcode, IEvent>();
        long requestId = 0;

        public ClientMessageScheduler(IClientTransport transport)
        {
            this.transport = transport;
        }

        long GetNextRequestId()
        {
            return Interlocked.Increment(ref requestId);
        }

        public Task Start()
        {
            transport.Connect();

            listenerTask = Task.Run(async () =>
            {
                while (!disposed)
                {
                    // Receive the server's reply
                    string? messageStr = await transport.ReceiveAsync();
                    if (messageStr == null)
                        break;

                    var frame = messageStr.ExtractFrameHeaders();
                    if (frame == null)
                    {
                        Console.WriteLine($"Client receive an invalid response message");
                        continue;
                    }
                    if (frame.options.HasFlag(TransportFrameHeaderOptions.ServerMsg))
                    {
                        HandleEvent(frame);
                    }
                    else if (pendingRequests.TryGetValue(frame.requestId, out Request? request))
                    {
                        request.PushResponse(frame);
                    }
                    else
                    {
                        Console.WriteLine($"Client no request found for this response {frame.requestId}");
                    }
                }
            });
            return listenerTask;
        }

        public bool RegisterEvent(Opcode messageType, IEvent eventAction)
        {
            return events.TryAdd(messageType, eventAction);
        }

        public bool UregisterEvent(Opcode messageType)
        {
            return events.TryRemove(messageType, out _);
        }

        private void HandleEvent(TransportFrameHeader frame)
        {
            if (!events.ContainsKey(frame.msgType))
            {
                Console.WriteLine($"Server no command found {frame.msgType}");
                return;
            }
            var cmd = events[frame.msgType];
            cmd.Execute(frame);
        }

        public async IAsyncEnumerable<T> SendLongRequest<T>(Func<long, string> buildPayload) where T : MessageHeader
        {
            var requestId = await SendNonBlock(buildPayload);
            TransportFrameHeader? reply = null;
            do
            {
                reply = await WaitForNextFrame(requestId);

                if (reply == null)
                {
                    throw new TimeoutException();
                    //yield break;
                }

                var response = reply.ExtractMessageHeaders<T>();
                if (response == null)
                {
                    throw new ArgumentNullException("response is invalid");
                }
                yield return response;
            } while (!reply.IsLastFrame());

        }

        public async Task<long> SendNonBlock(Func<long, string> buildPayload)
        {
            long newRequestId = GetNextRequestId();
            var payload = buildPayload(newRequestId);
            Request request = new Request(newRequestId, payload);
            await SendRequest(request, false);
            return newRequestId;
        }

        public Task<TransportFrameHeader?> Send(Func<long, string> buildPayload)
        {
            long newRequestId = GetNextRequestId();
            var payload = buildPayload(newRequestId);
            Request request = new Request(newRequestId, payload);
            return SendRequest(request);
        }

        public Task<TransportFrameHeader> WaitForNextFrame(long requestId)
        {
            var request = pendingRequests[requestId];
            return request.WaitForResponse((isLastFrame) => { if (isLastFrame) RemoveRequest(requestId); });
        }

        private Task<TransportFrameHeader?> SendRequest(Request request, bool waitForRespose = true)
        {
            if (pendingRequests.TryAdd(request.requestId, request))
            {
                transport.SendAsync(request.payload).Wait();
                if (waitForRespose)
                {
                    return WaitForNextFrame(request.requestId).
                        ContinueWith(task => (TransportFrameHeader?)task.Result); //convert to task nullable
                }
            }
            return Task.FromResult<TransportFrameHeader?>(null);
        }

        private void RemoveRequest(long requestId)
        {
            pendingRequests.TryRemove(requestId, out Request? request);
        }

        public void Dispose()
        {
            disposed = true;
            transport.Dispose();
        }
    }

}
