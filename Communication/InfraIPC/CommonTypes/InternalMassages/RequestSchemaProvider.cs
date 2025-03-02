namespace Intel.IntelConnect.IPC.CommonTypes
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
            var schema = _getSchema();
            if (schema.StartsWith("{") && schema.EndsWith("}"))
            {
                // Remove the first '{' and last '}'
                schema = schema.Substring(1, schema.Length - 2);
            }
            return $"{{\r\n  \"type\" : {_messageType},{schema}}}";
        }
    }
}
