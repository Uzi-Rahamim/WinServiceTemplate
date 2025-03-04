using Intel.IntelConnect.IPC.CommonTypes;

namespace Intel.IntelConnect.IPC.Extensions
{
    public static class FrameFlagExtensions
    {
        public static bool IsLastFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.LastFrame);
        }

        public static bool IsOpenSessionFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.OpenSessionMsg);
        }

        public static bool IsEventFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.EvantMsg);
        }

        public static bool IsRequestFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.RequestMsg);
        }

        public static bool IsResponseFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.ResponseMsg);
        }

       
        public static bool IsErrorFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.ErrorMsg);
        }
    }
}
