using Newtonsoft.Json;

namespace Intel.IntelConnect.IPC.Serializers
{
    public class JsonMessageSerializer
    {
        public static string SerializeObject<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T DeserializeObject<T>(string payload)
        {
            var obj = JsonConvert.DeserializeObject<T>(payload);
            if (obj == null)
            {
                throw new JsonException("Failed to deserialize message");
            }
            return obj;
        }
    }
}
