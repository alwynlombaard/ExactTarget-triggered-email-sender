using System;
using System.Collections.Generic;
using System.Linq;
using ExactTarget.TriggeredEmail.Core;
using ExactTarget.TriggeredEmail.Core.Configuration;
using ExactTarget.TriggeredEmail.Core.RequestClients.DataExtension;
using ExactTarget.TriggeredEmail.Core.RequestClients.Email;
using ExactTarget.TriggeredEmail.Core.RequestClients.EmailTemplate;
using ExactTarget.TriggeredEmail.Core.RequestClients.TriggeredSendDefinition;
using ExactTarget.TriggeredEmail.ExactTargetApi;

namespace ExactTarget.TriggeredEmail.Creation
{
    public class TriggeredEmailCreator : ITriggeredEmailCreator
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;
        private readonly ITriggeredSendDefinitionClient _triggeredSendDefinitionClient;
        private readonly IDataExtensionClient _dataExtensionClient;
        private readonly IEmailTemplateClient _emailTemplateClient;
        private readonly IEmailRequestClient _emailRequestClient;

        public TriggeredEmailCreator(IExactTargetConfiguration config, 
            IDataExtensionClient dataExtensionClient,
            ITriggeredSendDefinitionClient triggeredSendDefinitionClient,
            IEmailTemplateClient emailTemplateClient,
            IEmailRequestClient emailRequestClient)
        {
            _config = config;
            _dataExtensionClient = dataExtensionClient;
            _triggeredSendDefinitionClient = triggeredSendDefinitionClient;
            _emailTemplateClient = emailTemplateClient;
            _emailRequestClient = emailRequestClient;
        }

        public TriggeredEmailCreator(IExactTargetConfiguration config)
        {
            _client = SoapClientFactory.Manufacture(config);
            _config = config;
            _triggeredSendDefinitionClient = new TriggeredSendDefinitionClient(config);
            _dataExtensionClient = new DataExtensionClient(config);
            _emailTemplateClient = new EmailTemplateClient(config);
            _emailRequestClient = new EmailRequestClient(config);
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
                emailTemplateId = _emailTemplateClient.CreateEmailTemplate(emailTempalteExternalKey,
                                "template-" + externalKey,
                                "<body><custom type=\"content\" name=\"dynamicArea\"><custom name=\"opencounter\" type=\"tracking\">" +
                                "<table cellpadding=\"2\" cellspacing=\"0\" width=\"600\" ID=\"Table5\" Border=\"0\"><tr><td><font face=\"verdana\" size=\"1\" color=\"#444444\">This email was sent by: <b>%%Member_Busname%%</b><br>%%Member_Addr%% %%Member_City%%, %%Member_State%%, %%Member_PostalCode%%, %%Member_Country%%<br><br></font></td></tr></table>" + 
                                "</body>");
            }

            var emailName = "email-" + externalKey;
            var emailId = _emailRequestClient.CreateEmailFromTemplate(emailTemplateId,
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
    }
}