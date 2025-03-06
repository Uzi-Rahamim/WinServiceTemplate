using Intel.IntelConnect.IPC.CommonTypes;
using Intel.IntelConnect.IPC.Serializers;

namespace Intel.IntelConnect.IPC.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ExecuterAttribute<Rq, Rs> : Attribute where Rq : MessageHeader where Rs : MessageHeader
    {
        public string Name { get; }
        public string Schema { get; }

        // Constructor to pass data to the attribute
        public ExecuterAttribute(string name)
        {
            Name = name;
            Schema = JsonMessageSerializer.GetSchema<Rq, Rs>();
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ExecuterAttribute : Attribute 
    {
        public string Name { get; }

        // Constructor to pass data to the attribute
        public ExecuterAttribute(string name)
        {
            Name = name;
        }

        public static bool GetValues(Type type, out string? methodName, out string? schema)
        {
            var attribute = Attribute.GetCustomAttribute(type, typeof(ExecuterAttribute<,>)) ??
                                   Attribute.GetCustomAttribute(type, typeof(ExecuterAttribute));

            if (attribute == null)
                throw new Exception("Executer Attribute not found");

            methodName = (string?)attribute.GetType()?.GetProperty("Name")?.GetValue(attribute);
            schema = (string)(attribute.GetType()?.GetProperty("Schema")?.GetValue(attribute) ?? String.Empty);

            return true;
        }
    }
}
