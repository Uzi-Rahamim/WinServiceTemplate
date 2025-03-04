
namespace Intel.IntelConnect.IPC.CommonTypes
{

    public class RequestSchemaMessage : MessageHeader
    {
        public RequestSchemaMessage() : base(FrameworkMethodName.RequestSchema) { }
    }

    public class ResponseSchemaMessage : MessageHeader
    {
        public string schema { get; set; }
        public ResponseSchemaMessage(string schema) : base(FrameworkMethodName.RequestSchema) => this.schema = schema;
    }
}