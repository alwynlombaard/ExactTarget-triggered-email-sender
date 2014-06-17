using System;
using System.Collections.Generic;
using System.Linq;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail
{
    public class TriggeredEmailCreator : ITriggeredEmailCreator
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;

        public TriggeredEmailCreator(IExactTargetConfiguration config)
        {
            _config = config;
            _client = new SoapClient(_config.SoapBinding ?? "ExactTarget.Soap", _config.EndPoint);
            if (_client.ClientCredentials == null) return;
            _client.ClientCredentials.UserName.UserName = _config.ApiUserName;
            _client.ClientCredentials.UserName.Password = _config.ApiPassword;
        }

        public void Create(string externalKey, IEnumerable<string> dataExtensionFieldNames)
        {
            if (externalKey.Length > Guid.Empty.ToString().Length)
            {
                throw new ArgumentException("externalKey too long, should be max length of " + Guid.Empty.ToString().Length, "externalKey");
            }

            if (DoesTriggeredSendExist(externalKey))
            {
                throw new Exception(string.Format("A TriggeredSendDefinition with external key {0} already exsits", externalKey));
            }

            //if fields
            if (dataExtensionFieldNames.Any())
            {
                var templateId = RetrieveTriggeredSendDataExtensionTemplateId();
                //get or create dataextension
            }
            throw new System.NotImplementedException();
        }

        public bool DoesTriggeredSendExist(string externalKey)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                    ? new[] {new ClientID {ID = _config.ClientId.Value, IDSpecified = true}}
                    : null,
                ObjectType = "TriggeredSendDefinition",
                Properties = new[] {"Name", "ObjectID", "CustomerKey"},
                Filter = new SimpleFilterPart()
                {
                    Property = "CustomerKey",
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { externalKey }
                }
            };
            
            string requestId;
            APIObject[] results;

            _client.Retrieve(request, out requestId, out results);

            return results != null && results.Any();
        }

        private string RetrieveTriggeredSendDataExtensionTemplateId()
        {
            var request = new RetrieveRequest
            {
                ClientIDs =  _config.ClientId.HasValue 
                            ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } } 
                            : null,
                ObjectType =  "DataExtensionTemplate",
                Properties = new[] { "Name", "ObjectID", "CustomerKey" },
                Filter = new SimpleFilterPart()
                {
                    Property = "Name",
                    SimpleOperator = SimpleOperators.@equals,
                    Value = new[] { "TriggeredSendDataExtension" }
                }
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