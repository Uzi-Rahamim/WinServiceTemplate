using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.CommonTypes.InternalMassages;
using Intel.IntelConnect.IPC.Serializers;

namespace Intel.IntelConnect.IPC.Extensions
{
    public static class MessageExtensions
    {
        public static string ToJson<T>(this T obj)
        {
            return JsonMessageSerializer.SerializeObject(obj);
        }

        public static T FromJson<T>(this string payload) 
        {
            return JsonMessageSerializer.DeserializeObject<T>(payload); 
        }

        public static FrameHeader? ExtractFrameHeaders(this string messageJson)
        {
            return messageJson.FromJson<FrameHeader>();
        }

        public static T? ExtractMessageHeaders<T>(this FrameHeader frame) where T : MessageHeader
        {
            return frame.payload.FromJson<T>();
        }

        public static string BuildMessage<T>(this T message, long requestId, FrameOptions options) where T : MessageHeader
        {
            string payload = message.ToJson();
            return new FrameHeader(requestId, options, message.methodName, payload).ToJson();
        }

        public static string BuildOpenSessionRequestMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, FrameOptions.OpenSessionMsg | FrameOptions.RequestMsg);
        }

        public static string BuildRequestMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, FrameOptions.RequestMsg);
        }

        public static string BuildResponseMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, FrameOptions.ResponseMsg | FrameOptions.LastFrame);
        }

        public static string BuildErrorMessage<T>(this T message, long requestId) where T : ErrorMessage
        {
            return message.BuildMessage(requestId, FrameOptions.ResponseMsg | FrameOptions.ErrorMsg);
        }

        public static string BuildContinuingResponseMessage<T>(this T message, long requestId) where T : MessageHeader
        {
            return message.BuildMessage(requestId, FrameOptions.ResponseMsg);
        }

        public static string BuildServerEventMessage<T>(this T message) where T : MessageHeader
        {
            return message.BuildMessage(0, FrameOptions.EvantMsg);
        }
     
        public static bool IsNullMessage(this FrameHeader frame)
        {
            return frame.methodName.Contains(FrameworkMethodName.Empty);
        }
    }
}
