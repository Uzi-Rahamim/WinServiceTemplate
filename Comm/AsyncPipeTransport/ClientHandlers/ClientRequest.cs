using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using System.Threading.Channels;

namespace AsyncPipeTransport.ClientHandlers
{
    public class ClientRequest
    {
        private const int timeoutSeconds = 20;

        public long requestId { get; private set; }
        public string payload { get; private set; }
        private readonly TimeSpan timeout;
        private readonly Channel<FrameHeader> responseFrames;

        public ClientRequest(long requestId, string payload) : this(requestId, payload, TimeSpan.FromSeconds(timeoutSeconds))
        {
        }

        public ClientRequest(long requestId, string payload, TimeSpan timeout)
        {
            this.requestId = requestId;
            this.payload = payload;
            this.timeout = timeout;
            responseFrames = System.Threading.Channels.Channel.CreateUnbounded<FrameHeader>();
        }

        public void PushResponse(FrameHeader responseFrame)
        {
            responseFrames.Writer.TryWrite(responseFrame);
        }

        public async Task<FrameHeader> WaitForResponse(Action<bool> reset)
        {
            // Define the timeout task
            var timeoutTask = Task.Delay((int)timeout.TotalMilliseconds);
            // Define the read task
            var readTask = responseFrames.Reader.ReadAsync().AsTask();
            // Wait for the first task to complete (either the read or the timeout)
            var completedTask = await Task.WhenAny(readTask, timeoutTask);
            // If the read task completed, return the response frame
            if (completedTask == readTask)
            {
                // Item was successfully read
                var responseFrame = await readTask;
                reset(responseFrame.IsLastFrame());
                return responseFrame;
            }

            //timoout task completed
            reset(true);
            throw new TimeoutException();
        }
    }
}