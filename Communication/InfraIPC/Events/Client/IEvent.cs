using Intel.IntelConnect.IPC.CommonTypes;

namespace Intel.IntelConnect.IPC.Events.Client
{
    public interface IEvent
    {
        public void Execute(FrameHeader frame);
    }
}
