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

        public void Create(string externalKey, IEnumerable<string> dataExtensionFieldNames = null)
        {
            dataExtensionFieldNames = dataExtensionFieldNames ?? new List<string>();
            if (externalKey.Length > Guid.Empty.ToString().Length)
            {
                throw new ArgumentException("externalKey too long, should be max length of " + Guid.Empty.ToString().Length, "externalKey");
            }

            if (DoesTriggeredSendDefinitionExist(externalKey))
            {
                throw new Exception(string.Format("A TriggeredSendDefinition with external key {0} already exsits", externalKey));
            }

            
            //create data extension if needed
            var dataExtensionExternalKey = externalKey.ToLower().Reverse().ToString();
            if (!DoesDataExtensionExist(dataExtensionExternalKey))
            {
                var dataExtensionTemplateId = RetrieveTriggeredSendDataExtensionTemplateId();
                CreateDataExtension(_config.ClientId, 
                                    dataExtensionTemplateId, 
                                    dataExtensionExternalKey, 
                                    "TriggeredSend -" + externalKey, 
                                    dataExtensionFieldNames);
                
            }

            //create email template if needed
            var 

            //create email from template if needed


            //create triggeredSendDefinition

            throw new System.NotImplementedException();
        }

        public bool DoesTriggeredSendDefinitionExist(string externalKey)
        {
            return DoesObjectExist(externalKey, "TriggeredSendDefinition");
        }

        public bool DoesDataExtensionExist(string externalKey)
        {
            return DoesObjectExist(externalKey, "DataExtension");
        }

        public bool DoesEmailTemplateExist(string externalKey)
        {

            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                    ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                    : null,
                ObjectType = "Template",
                Properties = new[] { "TemplateName", "ObjectID", "CustomerKey" },
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

            return results != null && results.Any();
           
        }

        private bool DoesObjectExist(string externalKey, string objectType)
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
                Filter = new SimpleFilterPart
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

        private  void CreateDataExtension(int? clientId, 
                                            string dataExtensionTemplateId, 
                                            string externalKey,
                                            string name,
                                            IEnumerable<string> fields)
        {
            var de = new DataExtension
            {
                Client = clientId.HasValue ? new ClientID {ID = clientId.Value, IDSpecified = true} : null,
                Name = name,
                CustomerKey = externalKey,
                Template = new DataExtensionTemplate {ObjectID = dataExtensionTemplateId},
                Fields = fields.Select(field => new DataExtensionField
                {
                    Name = field,
                    FieldType = DataExtensionFieldType.Text,
                    FieldTypeSpecified = true,
                }).ToArray(),
            };

            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { de }, out requestId, out status);

            CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
        }

        public void CreateEmailTemplate(int? clientId,
            string externalKey,
            string name,
            string html)
        {
            var template = new Template
            {
                Client = clientId.HasValue ? new ClientID {ID = clientId.Value, IDSpecified = true} : null,
                TemplateName = name,
                CustomerKey = externalKey,
                LayoutHTML = html,
            };
            
            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { template }, out requestId, out status);

            CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject
        }
        
        
        private static void CheckResult(Result result)
        {
            if (result == null)
            {
                throw new Exception("Received an unexpected null result from ExactTarget");
            }

            if (result != null
                && !result.StatusCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception(string.Format("ExactTarget response indicates failure. StatusCode:{0} StatusMessage:{1}",
                                        result.StatusCode,
                                        result.StatusMessage));
            }
        }
    }
}