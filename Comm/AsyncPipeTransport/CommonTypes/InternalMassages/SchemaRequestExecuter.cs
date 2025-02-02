using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AsyncPipeTransport.CommonTypes
{
    public class SchemaRequestExecuter : BaseRequestExecuter<SchemaRequestExecuter, RequestSchemaMessage,ResponseSchemaMessage>
    {
        IEnumerable<IRequestSchemaProvider> _schemaProviderList;
        public SchemaRequestExecuter(ILogger<SchemaRequestExecuter> logger, CancellationTokenSource cts, IEnumerable<IRequestSchemaProvider> schemaProviderList) : 
            base(logger,cts) 
            => _schemaProviderList = schemaProviderList;

        protected override async Task<bool> Execute(RequestSchemaMessage requestMsg)
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
