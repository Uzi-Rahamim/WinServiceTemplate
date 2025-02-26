using AsyncPipeTransport.CommonTypes;

namespace AsyncPipeTransport.Extensions
{
    public static class FrameFlagExtensions
    {
        public static bool IsLastFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.LastFrame);
        }

        public static bool IsOpenSessionFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.OpenSession);
        }

        public static bool IsEventFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.EvantMsg);
        }

        public static bool IsRequestFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.Request);
        }

        public static bool IsResponseFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.Response);
        }

        public static bool IsDiscoveryFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.Discovery);
        }

        public static bool IsErrorFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameOptions.ErrorMsg);
        }
    }
}
