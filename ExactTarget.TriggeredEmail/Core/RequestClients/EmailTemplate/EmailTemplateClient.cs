using System.Linq;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core.RequestClients.EmailTemplate
{
    public class EmailTemplateClient : IEmailTemplateClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;

        public EmailTemplateClient(IExactTargetConfiguration config)
        {
            _client = SoapClientFactory.Manufacture(config);
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

        public int CreateEmailTemplate(string externalKey, string name, string html)
        {
            var template = new Template
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                TemplateName = name,
                CustomerKey = externalKey,
                LayoutHTML = html,
            };

            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { template }, out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject

            return result.First().NewID;
        }
    }
}
