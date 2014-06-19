using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core
{
    public interface IEmailTemplateClient
    {
        int RetrieveEmailTemplateId(string externalKey);
    }

    public class EmailTemplateClient : IEmailTemplateClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;

        public EmailTemplateClient(IExactTargetConfiguration config)
        {
            _client = ClientFactory.Manufacture(config);
            _config = config;
        }

        public int RetrieveEmailTemplateId(string externalKey)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                    ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                    : null,
                ObjectType = "Template",
                Properties = new[] { "ID", "TemplateName", "ObjectID", "CustomerKey" },
                Filter = new SimpleFilterPart
                {
                    Property = "CustomerKey",
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { externalKey }
                }
            };

            string requestId;
            APIObject[] results;
            _client.Retrieve(request, out requestId, out results);

            return results != null && results.Any() ? results.First().ID : 0;
        }
    }
}
