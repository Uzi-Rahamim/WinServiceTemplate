namespace AsyncPipeTransport.CommonTypes
{
    public class RequestSchemaProvider : IRequestSchemaProvider
    {
        private readonly Func<string> _getSchema;
        private readonly string _messageType;
        public RequestSchemaProvider(string messageType, Func<string> getSchema)
        {
            this._messageType = messageType;
            this._getSchema = getSchema;
        }

        public string GetSchema()
        {
            return $"{{\r\n type : {_messageType},\r\n schema : {_getSchema()} \r\n}}";
        }
    }
}
