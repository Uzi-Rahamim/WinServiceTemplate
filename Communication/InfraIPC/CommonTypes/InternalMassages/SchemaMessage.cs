
namespace Intel.IntelConnect.IPC.CommonTypes
{

    public class RequestSchemaMessage : MessageHeader
    {
        public RequestSchemaMessage() : base(FrameworkMessageTypes.RequestSchema) { }
    }

    public class ResponseSchemaMessage : MessageHeader
    {
        public string schema { get; set; }
        public ResponseSchemaMessage(string schema) : base(FrameworkMessageTypes.RequestSchema) => this.schema = schema;
    }
}