
namespace Intel.IntelConnect.IPC.CommonTypes
{

    public class RequestSchemaMessage : MessageHeader
    {
        public RequestSchemaMessage()  { }
    }

    public class ResponseSchemaMessage : MessageHeader
    {
        public string schema { get; set; }
        public ResponseSchemaMessage(string schema) => this.schema = schema;
    }
}