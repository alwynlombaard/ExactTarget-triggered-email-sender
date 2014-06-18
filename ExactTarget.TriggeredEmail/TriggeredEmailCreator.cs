using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ExactTarget.EmailFromTemplateCreator;
using ExactTarget.TriggeredEmail.ExactTargetApi;
using Result = ExactTarget.TriggeredEmail.ExactTargetApi.Result;

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

        public int Create(string externalKey)
        {
            var dataExtensionFieldNames = new HashSet<string>{"Subject", "Body"};
            if (externalKey.Length > Guid.Empty.ToString().Length)
            {
                throw new ArgumentException("externalKey too long, should be max length of " + Guid.Empty.ToString().Length, "externalKey");
            }

            if (DoesTriggeredSendDefinitionExist(externalKey))
            {
                throw new Exception(string.Format("A TriggeredSendDefinition with external key {0} already exsits", externalKey));
            }

            var dataExtensionExternalKey = GenerateExternalKey("data-extension-" + externalKey);
            if (!DoesDataExtensionExist(dataExtensionExternalKey))
            {
                var dataExtensionTemplateId = RetrieveTriggeredSendDataExtensionTemplateId();
                CreateDataExtension(_config.ClientId, 
                                    dataExtensionTemplateId, 
                                    dataExtensionExternalKey, 
                                    "triggeredsend-" + externalKey, 
                                    dataExtensionFieldNames);
                
            }

            var emailTempalteExternalKey = GenerateExternalKey("email-template" + externalKey);
            var emailTemplateId = RetrieveEmailTemplateId(emailTempalteExternalKey);

            if (emailTemplateId == 0)
            {
                emailTemplateId = CreateEmailTemplate(_config.ClientId,
                    emailTempalteExternalKey,
                    "template-" + externalKey,
                    "<custom type=\"content\" name=\"dynamicArea\"><custom name=\"opencounter\" type=\"tracking\">");
            }

            var emailName = "email-" + externalKey;
            var emailId = CreateEmailFromTemplate(emailTemplateId,
                    emailName,
                    "%%Subject%%",
                    new KeyValuePair<string, string>("dynamicArea", "%%Body%%"));
            
            return CreateTriggeredSendDefinition(_config.ClientId,
                externalKey,
                emailId,
                dataExtensionExternalKey,
                externalKey,
                externalKey);
        }

        public void StartTriggeredSend(string externalKey)
        {
            var ts = new TriggeredSendDefinition
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                CustomerKey = externalKey,
                TriggeredSendStatus = TriggeredSendStatusEnum.Active,
                TriggeredSendStatusSpecified = true
            };
           
            string requestId, overallStatus;
            var result = _client.Update(new UpdateOptions(), new APIObject[] { ts }, out requestId, out overallStatus);
            CheckResult(result.FirstOrDefault());
        }

        private string GenerateExternalKey(string value)
        {
            var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            return new Guid(bytes).ToString();
        }

        private bool DoesTriggeredSendDefinitionExist(string externalKey)
        {
            return DoesObjectExist(externalKey, "TriggeredSendDefinition");
        }

        private bool DoesDataExtensionExist(string externalKey)
        {
            return DoesObjectExist(externalKey, "DataExtension");
        }

        private int RetrieveEmailTemplateId(string externalKey)
        {
            var request = new RetrieveRequest
            {
                ClientIDs = _config.ClientId.HasValue
                    ? new[] { new ClientID { ID = _config.ClientId.Value, IDSpecified = true } }
                    : null,
                ObjectType = "Template",
                Properties = new[] {"ID", "TemplateName", "ObjectID", "CustomerKey" },
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

        private int CreateEmailTemplate(int? clientId,
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

            return result.First().NewID;
        }

        private int CreateTriggeredSendDefinition(int? clientId,
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

            CheckResult(result.FirstOrDefault());

            return result.First().NewID;

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

        private int CreateEmailFromTemplate(int emailTemplateId, string emailName, string subject, KeyValuePair<string, string> contentArea )
        {
            var emailCreator = new EmailCreator(new EmailFromTemplateCreator.ExactTargetConfiguration
            {
                ApiUserName = _config.ApiUserName,
                ApiPassword = _config.ApiPassword,
                ClientId = _config.ClientId,
                EndPoint = _config.EndPoint,
                SoapBinding = _config.SoapBinding
            });
            return emailCreator.Create(emailTemplateId, emailName, subject, contentArea);
        }
    }
}