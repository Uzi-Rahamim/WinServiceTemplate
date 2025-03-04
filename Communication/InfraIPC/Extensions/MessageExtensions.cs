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

        public static T? ExtractMessageHeaders<T>(this FrameHeader frame) where T : IMessageHeader
        {
            return frame.payload.FromJson<T>();
        }

        public static string BuildMessage<T>(this T message, string methodName, long requestId, FrameOptions options) where T : IMessageHeader
        {
            string payload = message.ToJson();
            return new FrameHeader(requestId, options, methodName, payload).ToJson();
        }

        public static string BuildOpenSessionRequestMessage<T>(this T message, long requestId) where T : IMessageHeader
        {
            return message.BuildMessage(FrameworkMethodName.Error, requestId, FrameOptions.OpenSessionMsg | FrameOptions.RequestMsg);
        }

        public static string BuildRequestMessage<T>(this T message, string methodName, long requestId) where T : IMessageHeader
        {
            return message.BuildMessage(methodName, requestId, FrameOptions.RequestMsg);
        }

        public static string BuildResponseMessage<T>(this T message, long requestId) where T : IMessageHeader
        {
            return message.BuildMessage(FrameworkMethodName.Empty, requestId,  FrameOptions.ResponseMsg | FrameOptions.LastFrame);
        }

        public static string BuildErrorMessage<T>(this T message, long requestId) where T : ErrorMessage
        {
            return message.BuildMessage(FrameworkMethodName.Empty, requestId, FrameOptions.ResponseMsg | FrameOptions.ErrorMsg);
        }

        public static string BuildContinuingResponseMessage<T>(this T message, long requestId) where T : IMessageHeader
        {
            return message.BuildMessage(FrameworkMethodName.Empty, requestId, FrameOptions.ResponseMsg);
        }

        public static string BuildServerEventMessage<T>(this T message) where T : IEventMessageHeader
        {
            return message.BuildMessage(message.topic, 0, FrameOptions.EvantMsg);
        }
     
        public static bool IsNullMessage(this FrameHeader frame)
        {
            return frame.methodName.Contains(FrameworkMethodName.Empty);
        }
    }
}
