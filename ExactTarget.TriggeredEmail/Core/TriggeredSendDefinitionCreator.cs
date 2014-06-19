using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core
{
    public interface ITriggeredSendDefinitionCreator
    {
        int CreateTriggeredSendDefinition(int? clientId,
            string externalId,
            int emailId, 
            string dataExtensionCustomerKey,
            string name, 
            string description);
    }

    public class TriggeredSendDefinitionCreator : ITriggeredSendDefinitionCreator
    {
        private readonly SoapClient _client;
        public TriggeredSendDefinitionCreator(IExactTargetConfiguration config)
        {
            _client = new SoapClient(config.SoapBinding ?? "ExactTarget.Soap", config.EndPoint);
            if (_client.ClientCredentials == null) return;
            _client.ClientCredentials.UserName.UserName = config.ApiUserName;
            _client.ClientCredentials.UserName.Password = config.ApiPassword;
        }

        public int CreateTriggeredSendDefinition(int? clientId,
            string externalId,
            int emailId, 
            string dataExtensionCustomerKey,
            string name, 
            string description)
        {
            var ts = new TriggeredSendDefinition
            {
                Client = clientId.HasValue ? new ClientID { ID = clientId.Value, IDSpecified = true } : null,
                Email = new Email { ID = emailId, IDSpecified = true },
                SendSourceDataExtension = new DataExtension { CustomerKey = dataExtensionCustomerKey },
                Name = name,
                Description = description,
                CustomerKey = externalId,
                TriggeredSendStatus = TriggeredSendStatusEnum.Active,
                SendClassification = new SendClassification
                {
                    CustomerKey = "Default Transactional"
                },
                IsMultipart = true,
                IsMultipartSpecified = true,

            };
            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { ts }, out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());

            return result.First().NewID;

        }
    }
}