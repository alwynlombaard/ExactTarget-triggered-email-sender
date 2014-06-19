using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Core
{
    public class SharedCoreRequestClient : ISharedCoreRequestClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;

        public SharedCoreRequestClient(IExactTargetConfiguration config)
        {
            _client = ClientFactory.Manufacture(config);
            _config = config;
        }

        public bool DoesObjectExist(string propertyName, string value, string objectType)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                    ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                    : null,
                ObjectType = objectType,
                Properties = new[] { "Name", "ObjectID", "CustomerKey" },
                Filter = new SimpleFilterPart
                {
                    Property = propertyName,
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { value }
                }
            };

            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            return results != null && results.Any();
        }
    }
}
