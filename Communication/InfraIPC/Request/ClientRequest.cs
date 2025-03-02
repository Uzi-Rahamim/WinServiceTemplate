using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Extensions;
using System.Threading.Channels;

namespace Intel.IntelConnect.IPC.Request
{
    public class ClientRequest
    {
        private const int timeoutSeconds = 20;

        public long requestId { get; private set; }
        public string payload { get; private set; }
        private readonly TimeSpan _timeout;
        private readonly Channel<FrameHeader> _responseFrames;
        private readonly CancellationToken _cancellationToken;

        public ClientRequest(long requestId, string payload, CancellationToken cancellationToken) : this(requestId, payload, cancellationToken, TimeSpan.FromSeconds(timeoutSeconds))
        {
        }

        public ClientRequest(long requestId, string payload, CancellationToken cancellationToken, TimeSpan timeout)
        {
            this.requestId = requestId;
            this.payload = payload;
            this._timeout = timeout;
            this._cancellationToken = cancellationToken;
            _responseFrames = System.Threading.Channels.Channel.CreateUnbounded<FrameHeader>();
        }

        public void PushResponse(FrameHeader responseFrame)
        {
            _responseFrames.Writer.TryWrite(responseFrame);
        }

        public async Task<FrameHeader> WaitForResponse(Action<bool> reset) => await WaitForResponse(reset, _timeout);


        public async Task<FrameHeader> WaitForResponse(Action<bool> reset, TimeSpan timeout)
        {
            try
            {
                // Define the timeout task
                var timeoutTask = Task.Delay((int)_timeout.TotalMilliseconds, _cancellationToken);
                // Define the read task
                var readTask = _responseFrames.Reader.ReadAsync().AsTask();
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

                if (_cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                //timoout task completed
                throw new TimeoutException();
            }
            catch (Exception) 
            {
                reset(true);
                throw; //this will throw TimeoutException & TaskCanceledException
            }
        }
    }
}