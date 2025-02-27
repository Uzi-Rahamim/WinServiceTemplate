using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;

namespace AsyncPipeTransport.CommonTypes.InternalMassages.Executers
{
    public class SchemaRequestExecuter : StreamResponseRequestExecuter<SchemaRequestExecuter, RequestSchemaMessage,ResponseSchemaMessage>
    {
        IEnumerable<IRequestSchemaProvider> _schemaProviderList;
        public SchemaRequestExecuter(ILogger<SchemaRequestExecuter> logger, CancellationTokenSource cts, IEnumerable<IRequestSchemaProvider> schemaProviderList) : 
            base(logger,cts) 
            => _schemaProviderList = schemaProviderList;

        protected override async IAsyncEnumerable<ResponseSchemaMessage> Execute(RequestSchemaMessage requestMsg)
        {
            // Simulate async work
            await Task.Yield();

            yield return new ResponseSchemaMessage("{ commands : [");
            foreach (var schemaProvider in _schemaProviderList)
            {
                yield return new ResponseSchemaMessage(schemaProvider.GetSchema());
            }
            yield return new ResponseSchemaMessage("]}");
        }
    }

}
