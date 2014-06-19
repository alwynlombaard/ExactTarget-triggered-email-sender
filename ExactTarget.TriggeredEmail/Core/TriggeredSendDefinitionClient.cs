using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core
{
    public class TriggeredSendDefinitionClient : ITriggeredSendDefinitionClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;
        private readonly ISharedCoreRequestClient _sharedCoreRequestClient;

        public TriggeredSendDefinitionClient(IExactTargetConfiguration config)
        {
            _config = config;
            _client = new SoapClient(config.SoapBinding ?? "ExactTarget.Soap", config.EndPoint);
            if (_client.ClientCredentials == null) return;
            _client.ClientCredentials.UserName.UserName = config.ApiUserName;
            _client.ClientCredentials.UserName.Password = config.ApiPassword;
            _sharedCoreRequestClient = new SharedCoreRequestClient(config);
        }

        public int CreateTriggeredSendDefinition(
            string externalId,
            int emailId, 
            string dataExtensionCustomerKey,
            string name, 
            string description)
        {
            var ts = new TriggeredSendDefinition
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
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

        public bool DoesTriggeredSendDefinitionExist(string externalKey)
        {
            return _sharedCoreRequestClient.DoesObjectExist("CustomerKey", externalKey, "TriggeredSendDefinition");
        }

        
    }
}