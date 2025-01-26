using AsyncPipeTransport.ServerHandlers;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AsyncPipeTransport.CommonTypes
{
    public class SchemaRequestHandler : BaseRequestExecuter<SchemaRequestHandler, RequestSchemaMessage>
    {
        IEnumerable<IRequestSchemaProvider> _schemaProviderList;
        public SchemaRequestHandler(ILogger<SchemaRequestHandler> logger, IEnumerable<IRequestSchemaProvider> schemaProviderList) : base(logger) 
            => _schemaProviderList = schemaProviderList;

        protected override async Task<bool> ExecuteInternal(RequestSchemaMessage requestMsg)
        {
            StringBuilder sb = new();
            await SendContinuingResponse<ResponseSchemaMessage>(new ResponseSchemaMessage("{ commands : ["));
            foreach (var schemaProvider in _schemaProviderList)
            {
                await SendContinuingResponse<ResponseSchemaMessage>(new ResponseSchemaMessage(schemaProvider.GetSchema()));
            }
            await SendLastResponse<ResponseSchemaMessage>(new ResponseSchemaMessage("]}"));
            return true;
        }
    }

}
