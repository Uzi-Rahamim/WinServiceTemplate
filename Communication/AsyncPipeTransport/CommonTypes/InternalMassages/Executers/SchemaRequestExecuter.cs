using AsyncPipeTransport.Executer;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AsyncPipeTransport.CommonTypes.InternalMassages.Executers
{
    public class SchemaRequestExecuter : BaseRequestExecuter<SchemaRequestExecuter, RequestSchemaMessage,ResponseSchemaMessage>
    {
        IEnumerable<IRequestSchemaProvider> _schemaProviderList;
        public SchemaRequestExecuter(ILogger<SchemaRequestExecuter> logger, CancellationTokenSource cts, IEnumerable<IRequestSchemaProvider> schemaProviderList) : 
            base(logger,cts) 
            => _schemaProviderList = schemaProviderList;

        protected override async Task<ResponseSchemaMessage?> Execute(
            RequestSchemaMessage requestMsg,
            Func<ResponseSchemaMessage, Task> sendPage)
        {
            StringBuilder sb = new();
            await sendPage(new ResponseSchemaMessage("{ commands : ["));
            foreach (var schemaProvider in _schemaProviderList)
            {
                await sendPage(new ResponseSchemaMessage(schemaProvider.GetSchema()));
            }
            return new ResponseSchemaMessage("]}");
        }
    }

}
