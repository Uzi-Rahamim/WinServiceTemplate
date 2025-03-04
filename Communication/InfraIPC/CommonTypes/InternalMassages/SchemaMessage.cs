
namespace Intel.IntelConnect.IPC.CommonTypes
{

    public class RequestSchemaMessage : IMessageHeader
    {
        public RequestSchemaMessage()  { }
    }

    public class ResponseSchemaMessage : IMessageHeader
    {
        public string schema { get; set; }
        public ResponseSchemaMessage(string schema) => this.schema = schema;
    }
}