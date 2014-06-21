using System.Collections.Generic;
using ExactTarget.EmailFromTemplateCreator;
using ExactTarget.TriggeredEmail.ExactTargetApi;
using IExactTargetConfiguration = ExactTarget.TriggeredEmail.Core.Configuration.IExactTargetConfiguration;
using System.Linq;

namespace ExactTarget.TriggeredEmail.Core.RequestClients.Email
{
    public class EmailRequestClient : IEmailRequestClient
    {
        private readonly IExactTargetConfiguration _config;
        private readonly SoapClient _client;
        private readonly IEmailCreator _emailFromTemplateCreator;


        public EmailRequestClient(IExactTargetConfiguration config, IEmailCreator emailFromTemplateCreator)
        {
            _config = config;
            _client = SoapClientFactory.Manufacture(config);
            _emailFromTemplateCreator = emailFromTemplateCreator;
        }

        public EmailRequestClient(IExactTargetConfiguration config)
        {
            _config = config;
            _client = SoapClientFactory.Manufacture(config);
            _emailFromTemplateCreator = new EmailCreator(new ExactTargetConfiguration
            {
                ApiUserName = _config.ApiUserName,
                ApiPassword = _config.ApiPassword,
                ClientId = _config.ClientId,
                EndPoint = _config.EndPoint,
                SoapBinding = _config.SoapBinding
            });
        }

        public int CreateEmailFromTemplate(int emailTemplateId, string emailName, string subject, KeyValuePair<string, string> contentArea)
        {
            return _emailFromTemplateCreator.Create(emailTemplateId, emailName, subject, contentArea);
        }

        public int CreateEmail(string externalKey, string emailName, string subject, string htmlBody)
        {
            var email = new ExactTargetApi.Email
            {
                Client = _config.ClientId.HasValue ? new ClientID { ID = _config.ClientId.Value, IDSpecified = true } : null,
                Name = emailName,
                CustomerKey = externalKey,
                IsHTMLPaste = true,
                IsHTMLPasteSpecified = true,
                SyncTextWithHTML = true,
                SyncTextWithHTMLSpecified = true,
                HTMLBody = htmlBody,
                Subject = subject,
                CharacterSet = "UTF-8"
            };

            string requestId, status;
            var result = _client.Create(new CreateOptions(), new APIObject[] { email }, out requestId, out status);

            ExactTargetResultChecker.CheckResult(result.FirstOrDefault()); //we expect only one result because we've sent only one APIObject

            return result.First().NewID;
        }
    }
}
