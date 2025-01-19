using AsyncPipeTransport.CommonTypes;
using AsyncPipeTransport.Extensions;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AsyncPipeTransport.ClientDistributer
{

    public class Request
    {
        private const int timeoutSeconds = 20;

        public long requestId { get; private set; }
        public string payload { get; private set; }
        private readonly TimeSpan timeout;
        private readonly Channel<TransportFrameHeader> responseFrames;

        public Request(long requestId, string payload) : this(requestId, payload, TimeSpan.FromSeconds(timeoutSeconds))
        {

        }

        public Request(long requestId, string payload, TimeSpan timeout)
        {
            this.requestId = requestId;
            this.payload = payload;
            this.timeout = timeout;
            responseFrames = Channel.CreateUnbounded<TransportFrameHeader>();
        }

        public void PushResponse(TransportFrameHeader responseFrame)
        {
            responseFrames.Writer.TryWrite(responseFrame);
        }



        public async Task<TransportFrameHeader> WaitForResponse(Action<bool> reset)
        {
            // Define the timeout task
            var timeoutTask = Task.Delay((int) timeout.TotalMilliseconds);
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
            reset(true);
            throw new TimeoutException();
        }
            
    }



    //public class Request
    //{
    //    private const int timeoutSeconds = 20;

    //    public long requestId { get; private set; }
    //    public string payload { get; private set; }

    //    private readonly BlockingCollection<TransportFrameHeader> responseFrames = new BlockingCollection<TransportFrameHeader>();
    //    private readonly TimeSpan timeout;

    //    public Request(long requestId, string payload) : this(requestId, payload, TimeSpan.FromSeconds(timeoutSeconds))
    //    {

    //    }

    //    public Request(long requestId, string payload, TimeSpan timeout)
    //    {
    //        this.requestId = requestId;
    //        this.payload = payload;
    //        this.timeout = timeout;
    //    }

    //    public void PushResponse(TransportFrameHeader responseFrame)
    //    {
    //        responseFrames.Add(responseFrame);
    //    }

    //    public async Task<TransportFrameHeader> WaitForResponse(Action<bool> reset)
    //    {
    //        var succeed = responseFrames.TryTake(out TransportFrameHeader? responseFrame, (int)timeout.TotalMilliseconds);
    //        if (succeed && responseFrame != null)
    //        {
    //            reset(responseFrame.IsLastFrame());
    //            return responseFrame;
    //        }
    //        reset(true);
    //        throw new TimeoutException();
    //    }
    //}
}