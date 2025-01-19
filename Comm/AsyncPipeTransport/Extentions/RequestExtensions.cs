using AsyncPipeTransport.CommonTypes;
using Newtonsoft.Json;

namespace AsyncPipeTransport.Extensions
{
    public static class RequestExtensions
    {
        public static string ToJson<T>(this T obj)
        {
            // Serialize the record to JSON
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

        public static TransportFrameHeader? ExtractFrameHeaders(this string messageJson)
        {
            return JsonConvert.DeserializeObject<TransportFrameHeader>(messageJson);
        }

        public static T? ExtractMessageHeaders<T>(this TransportFrameHeader frame) where T : MessageHeader
        {
            return JsonConvert.DeserializeObject<T>(frame.payload);
        }


        public static string BuildRequestMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, TransportFrameHeaderOptions.LastFrame);
        }

        public static string BuildResponseMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, TransportFrameHeaderOptions.LastFrame);
        }

        public static string BuildContinuingResponseMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, TransportFrameHeaderOptions.None);
        }

        public static string BuildServerEventMessage<T>(this T message) where T : MessageHeader
        {
            return message.BuildMessage(0, TransportFrameHeaderOptions.ServerMsg);
        }

        public static string BuildMessage<T>(this T message, long requestId, TransportFrameHeaderOptions options) where T : MessageHeader
        {
            string payload = message.ToJson();
            return new TransportFrameHeader(requestId, options, message.msgType, payload).ToJson();
        }

       

        public static bool IsLastFrame(this TransportFrameHeader frame)
        {
            return frame.options.HasFlag(TransportFrameHeaderOptions.LastFrame);
        }


    }
}
