using AsyncPipeTransport.CommonTypes;
using Newtonsoft.Json;

namespace AsyncPipeTransport.Extensions
{
    public static class RequestExtensions
    {
        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(this string payload) where T : MessageHeader
        {
            var obj = JsonConvert.DeserializeObject<T>(payload);
            if (obj == null)
            {
                throw new JsonException("Failed to deserialize message payload");
            }
            return obj;
        }

        public static FrameHeader? ExtractFrameHeaders(this string messageJson)
        {
            return JsonConvert.DeserializeObject<FrameHeader>(messageJson);
        }

        public static T? ExtractMessageHeaders<T>(this FrameHeader frame) where T : MessageHeader
        {
            return JsonConvert.DeserializeObject<T>(frame.payload);
        }


        public static string BuildRequestMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, FrameHeaderOptions.LastFrame);
        }

        public static string BuildResponseMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, FrameHeaderOptions.LastFrame);
        }

        public static string BuildContinuingResponseMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, FrameHeaderOptions.None);
        }

        public static string BuildServerEventMessage<T>(this T message) where T : MessageHeader
        {
            return message.BuildMessage(0, FrameHeaderOptions.EvantMsg);
        }

        public static string BuildMessage<T>(this T message, long requestId, FrameHeaderOptions options) where T : MessageHeader
        {
            string payload = message.ToJson();
            return new FrameHeader(requestId, options, message.msgType, payload).ToJson();
        }

        public static bool IsLastFrame(this FrameHeader frame)
        {
            return frame.options.HasFlag(FrameHeaderOptions.LastFrame);
        }
    }
}
