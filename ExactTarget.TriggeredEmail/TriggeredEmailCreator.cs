using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail
{
    public class TriggeredEmailCreator : ITriggeredEmailCreator
    {
        private readonly IExactTargetConfiguration _config;
        private SoapClient _client;

        public TriggeredEmailCreator(IExactTargetConfiguration config)
        {
            _config = config;
            _client = new SoapClient(_config.SoapBinding ?? "ExactTarget.Soap", _config.EndPoint);
        }

        public void Create()
        {
            throw new System.NotImplementedException();
        }
        
        private  string RetrieveDataExtensionTemplateId()
        {
            var request = new RetrieveRequest
            {
                ClientIDs = new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } },
                ObjectType = "DataExtensionTemplate",
                Properties = new[] { "Name", "ObjectID", "CustomerKey" }
            };
            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);


            if (results != null && results.Any())
            {
                return results.First().ObjectID;
            }

            return string.Empty;
        }
    }
}