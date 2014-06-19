using System.Collections.Generic;
using System.Linq;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.Core.RequestClients.Shared;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core.RequestClients.DataExtension
{
    public class DataExtensionClient : IDataExtensionClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;
        private readonly SharedCoreRequestClient _sharedCoreRequestClient;

        public DataExtensionClient(IExactTargetConfiguration config)
        {
            _config = config;
            _client = SoapClientFactory.Manufacture(config);
            _sharedCoreRequestClient = new SharedCoreRequestClient(config);
        }

        public void CreateDataExtension(string dataExtensionTemplateObjectId,
                                           string externalKey,
                                           string name,
                                           IEnumerable<string> fields)
        {
            var de = new ExactTargetApi.DataExtension
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                Name = name,
                CustomerKey = externalKey,
                Template = new DataExtensionTemplate { ObjectID = dataExtensionTemplateObjectId },
                Fields = fields.Select(field => new DataExtensionField
                {
                    Name = field,
                    FieldType = DataExtensionFieldType.Text,
                    FieldTypeSpecified = true,
                }).ToArray(),
            };

            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { de }, out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
        }

        public bool DoesDataExtensionExist(string externalKey)
        {
            return _sharedCoreRequestClient.DoesObjectExist("CustomerKey", externalKey, "DataExtension");
        }

        public string RetrieveTriggeredSendDataExtensionTemplateObjectId()
        {
            return _sharedCoreRequestClient.RetrieveObjectId("Name", "TriggeredSendDataExtension", "DataExtensionTemplate");
        }
    }
}
