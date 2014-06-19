using System;
using System.Collections.Generic;
using System.Linq;
using ExactTarget.EmailFromTemplateCreator;
using ExactTarget.TriggeredEmail.Core;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail
{
    public class TriggeredEmailCreator : ITriggeredEmailCreator
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;
        private readonly ITriggeredSendDefinitionClient _triggeredSendDefinitionClient;
        private readonly IDataExtensionClient _dataExtensionClient;
        private readonly IEmailTemplateClient _emailTemplateClient;

        public TriggeredEmailCreator(IExactTargetConfiguration config, 
            IDataExtensionClient dataExtensionClient,
            ITriggeredSendDefinitionClient triggeredSendDefinitionClient,
            IEmailTemplateClient emailTemplateClient)
        {
            _config = config;
            _dataExtensionClient = dataExtensionClient;
            _triggeredSendDefinitionClient = triggeredSendDefinitionClient;
            _emailTemplateClient = emailTemplateClient;
        }

        public TriggeredEmailCreator(IExactTargetConfiguration config)
        {
            _client = ClientFactory.Manufacture(config);
            _config = config;
            _triggeredSendDefinitionClient = new TriggeredSendDefinitionClient(config);
            _dataExtensionClient = new DataExtensionClient(config);
            _emailTemplateClient = new EmailTemplateClient(config);
        }

        public int Create(string externalKey)
        {
            
            if (externalKey.Length > Guid.Empty.ToString().Length)
            {
                throw new ArgumentException("externalKey too long, should be max length of " + Guid.Empty.ToString().Length, "externalKey");
            }

            if (_triggeredSendDefinitionClient.DoesTriggeredSendDefinitionExist(externalKey))
            {
                throw new Exception(string.Format("A TriggeredSendDefinition with external key {0} already exsits", externalKey));
            }

            var dataExtensionExternalKey = ExternalKeyGenerator.GenerateExternalKey("data-extension-" + externalKey);
            if (!_dataExtensionClient.DoesDataExtensionExist(dataExtensionExternalKey))
            {
                var dataExtensionTemplateObjectId = _dataExtensionClient.RetrieveTriggeredSendDataExtensionTemplateObjectId();
                var dataExtensionFieldNames = new HashSet<string> { "Subject", "Body" };
                _dataExtensionClient.CreateDataExtension(_config.ClientId, 
                                    dataExtensionTemplateObjectId, 
                                    dataExtensionExternalKey, 
                                    "triggeredsend-" + externalKey, 
                                    dataExtensionFieldNames);
                
            }

            var emailTempalteExternalKey = ExternalKeyGenerator.GenerateExternalKey("email-template" + externalKey);
            var emailTemplateId = _emailTemplateClient.RetrieveEmailTemplateId(emailTempalteExternalKey);

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
            
            return _triggeredSendDefinitionClient.CreateTriggeredSendDefinition(externalKey,
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
            ExactTargetResultChecker.CheckResult(result.FirstOrDefault());
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

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject

            return result.First().NewID;
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