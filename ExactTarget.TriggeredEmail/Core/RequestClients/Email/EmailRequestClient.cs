using System.Collections.Generic;
using ExactTarget.EmailFromTemplateCreator;
using ExactTarget.TriggeredEmail.ExactTargetApi;
using IExactTargetConfiguration = ExactTarget.TriggeredEmail.Core.Configuration.IExactTargetConfiguration;

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
            _emailFromTemplateCreator = new EmailCreator(new EmailFromTemplateCreator.ExactTargetConfiguration
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
    }
}
