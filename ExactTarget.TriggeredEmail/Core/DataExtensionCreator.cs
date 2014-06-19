using System.Collections.Generic;
using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core
{
    public interface IDataExtensionCreator
    {
        void CreateDataExtension(int? clientId,
            string dataExtensionTemplateId,
            string externalKey,
            string name,
            IEnumerable<string> fields);
    }

    public class DataExtensionCreator : IDataExtensionCreator
    {
       private readonly SoapClient _client;

        public DataExtensionCreator(IExactTargetConfiguration config)
        {
            _client = new SoapClient(config.SoapBinding ?? "ExactTarget.Soap", config.EndPoint);
            if (_client.ClientCredentials == null) return;
            _client.ClientCredentials.UserName.UserName = config.ApiUserName;
            _client.ClientCredentials.UserName.Password = config.ApiPassword;
        }

        public void CreateDataExtension(int? clientId,
                                           string dataExtensionTemplateId,
                                           string externalKey,
                                           string name,
                                           IEnumerable<string> fields)
        {
            var de = new DataExtension
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                Name = name,
                CustomerKey = externalKey,
                Template = new DataExtensionTemplate { ObjectID = dataExtensionTemplateId },
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

    }
}
