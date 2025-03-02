namespace Intel.IntelConnect.IPC.Utils
{
    public class SequenceGenerator : ISequenceGenerator
    {
        private long _requestId = 0;
        public long GetNextId()
        {
            return Interlocked.Increment(ref _requestId);
        }
    }
}
