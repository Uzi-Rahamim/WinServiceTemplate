using Newtonsoft.Json;
using System.Reflection;

namespace Intel.IntelConnect.IPC.Serializers
{
    internal class JsonMessageSerializer
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

        public static string GetSchema<Rq,Rs>()
        {
            var properties = new
            {
                request = new
                {
                    name = typeof(Rq).Name,
                    properties = typeof(Rq).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new
                    {
                        name = p.Name,
                        type = p.PropertyType.Name
                    }).ToList() // Convert PropertyInfo to a simple structure
                },
                response = new
                {
                    name = typeof(Rs).Name,
                    properties = typeof(Rs).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new
                    {
                        name = p.Name,
                        type = p.PropertyType.Name
                    }).ToList() // Convert PropertyInfo to a simple structure
                }
            };

            // Convert the type information (properties) to JSON
            return JsonConvert.SerializeObject(properties, Formatting.Indented);
        }
    }
}
